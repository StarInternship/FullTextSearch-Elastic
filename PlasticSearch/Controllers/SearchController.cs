using PlasticSearch.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PlasticSearch.Controllers
{
    public class SearchController
    {
        public static SearchController Instance { get; } = new SearchController();
        private ISet<string> result;
        private readonly Stopwatch sw = new Stopwatch();
        private long preprocessTime = -1;
        private Task preprocessTask;

        public void Preprocess()
        {
            preprocessTask = new Task(() =>
            {
                sw.Start();
                Importer.CreateLog();

                Importer importer = new Importer();

                importer.ReadFiles();

                DatabaseController.Instance.InsertFiles();

                sw.Stop();
                preprocessTime = sw.ElapsedMilliseconds;
            });
            preprocessTask.Start();
        }


        public SearchResult Search(string query, string type)
        {
            return new SearchResult(result, 0);
        }

        public long GetIsReady()
        {
            preprocessTask.Wait();

            return preprocessTime;
        }
    }
}