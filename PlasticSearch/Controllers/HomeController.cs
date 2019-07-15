using PlasticSearch.Models;
using PlasticSearch.Models.search;
using PlasticSearch.Models.tokenizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace PlasticSearch.Controllers
{
    public class HomeController : Controller
    {

        private static readonly Tokenizer exactSearchTokenizer = new ExactSearchTokenizer();
        private static readonly Tokenizer ngramSearchTokenizer = new NgramSearchTokenizer();
        private static readonly Search search = new Search();
        private static ISet<string> result;
        private static ISet<string> queryTokens;
        private static readonly Stopwatch sw = new Stopwatch();
        private static long preprocessTime = -1;
        private static Thread preprocessThread;
        private static Dictionary<string, string> files;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(string query)
        {
            result = new HashSet<string>();

            QueryProcess(query);
            long searchTime = DoSearch();

            return Json(new SearchResult(result, searchTime));
        }


        static void QueryProcess(string query)
        {
            sw.Restart();

            queryTokens = exactSearchTokenizer.TokenizeQuery(ngramSearchTokenizer.CleanText(query));

        }

        static long DoSearch()
        {

            search.search(queryTokens.ToList(), result);

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }


        public static void Preprocess()
        {
            preprocessThread = new Thread(() =>
            {
                sw.Start();

                Importer importer = new Importer();

                files = importer.ReadFiles();

                sw.Start();

                foreach (var pair in files)
                {
                    string cleanText = ngramSearchTokenizer.CleanText(pair.Value);
                    ngramSearchTokenizer.tokenizeData(pair.Key, cleanText, search.NgramData);
                    exactSearchTokenizer.tokenizeData(pair.Key, cleanText, search.ExactData);
                }



                sw.Stop();
                preprocessTime = sw.ElapsedMilliseconds;
            });
            preprocessThread.Start();

        }



        [HttpPost]
        public long IsReady()
        {
            preprocessThread.Join();

            return preprocessTime;
        }

        [HttpPost]
        public JsonResult GetFiles()
        {
            preprocessThread.Join();

            return Json(new SearchResult(new HashSet<string>(files.Keys), 1543));
        }
    }
}