using Nest;
using System;
using System.Collections.Generic;
using PlasticSearch.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private readonly Stopwatch sw = new Stopwatch();
        private long preprocessTime = -1;
        private Task preprocessTask;
        private List<Text> files = new List<Text>();
        private ElasticClient client;

        public void Preprocess()
        {
            preprocessTask = new Task(() =>
            {

                Importer.CreateLog();
                sw.Start();
                Importer.WriteLog("starting...");
                Connect();

                Importer.WriteLog("connect " + sw.ElapsedMilliseconds + " ms");


                Importer importer = new Importer();
                importer.ReadFiles();

                Importer.WriteLog("read files in " + sw.ElapsedMilliseconds + " ms");

                InsertFiles();

                Importer.WriteLog("insert in " + sw.ElapsedMilliseconds + " ms");

                sw.Stop();
                preprocessTime = sw.ElapsedMilliseconds;
            });
            preprocessTask.Start();
        }

        public void Connect()
        {
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("texts");
            client = new ElasticClient(settings);
            client.DeleteByQueryAsync<Text>(del => del.Query(q => q.MatchAll())).Wait();
        }

        public void AddFile(string fileName, string text)
        {
            files.Add(new Text { fileName = fileName, text = text });
        }

        public void InsertFiles()
        {
            var bulkIndexResponse = client.BulkAsync(b => b
            .Index("texts")
            .IndexMany(files)
            .Refresh(Elasticsearch.Net.Refresh.True)
            );
            bulkIndexResponse.Wait();
            files.Clear();
        }

        public SearchResult Search(string query, string type)
        {
            var searchResponse = client.Search<Text>(s => s.Query(q => q.Match(m => m.Field(f => f.text).Query(query))));
            var texts = searchResponse.Documents;

            return new SearchResult(new HashSet<Text>(texts).Select(x => x.fileName), searchResponse.Took);
        }

        public long IsReady()
        {
            preprocessTask.Wait();

            return preprocessTime;
        }
    }

    class Text
    {
        public string text { get; set; }
        public string fileName { get; set; }

        public override int GetHashCode()
        {
            return (text + " ->" + fileName).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
    }
}