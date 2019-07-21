using Nest;
using System;
using System.Collections.Generic;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private List<Text> files = new List<Text>();
        private ElasticClient client;

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

        public List<string> search(string text)
        {
            return new List<string>();
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