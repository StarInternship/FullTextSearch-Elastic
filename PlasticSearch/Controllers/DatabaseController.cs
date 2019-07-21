using Nest;
using System;
using System.Collections.Generic;
using PlasticSearch.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private ISet<string> result;
        private readonly Stopwatch sw = new Stopwatch();
        private long preprocessTime = -1;
        private Task preprocessTask;
        private List<Text> files = new List<Text>();
        private ElasticClient client;

        public void Preprocess()
        {
            preprocessTask = new Task(() =>
            {
                sw.Start();
                Importer.CreateLog();

                Importer importer = new Importer();

                importer.ReadFiles();

                InsertFiles();

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

        }

        public SearchResult Search(string query, string type)
        {
            return new SearchResult(result, 0);
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