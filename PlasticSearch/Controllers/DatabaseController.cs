using Nest;
using PlasticSearch.Models;
using System.Collections.Generic;
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
        private LinkedList<Text> files = new LinkedList<Text>();
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

        }

        public void AddFile(string fileName, string text)
        {

        }

        public void InsertFiles()
        {

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