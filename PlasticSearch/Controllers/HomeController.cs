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


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(string query)
        {
            return Json(new SearchResult(new HashSet<string>(), 2000));
        }

        public static void Preprocess()
        {
            Importer importer = new Importer();

            IDictionary<string, string> files = importer.ReadFiles();

            sw.Start();

            foreach (var pair in files)
            {
                string cleanText = ngramSearchTokenizer.CleanText(pair.Value);
                ngramSearchTokenizer.tokenizeData(pair.Key, cleanText, search.NgramData);
                exactSearchTokenizer.tokenizeData(pair.Key, cleanText, search.ExactData);
            }

            sw.Stop();
            preprocessTime = sw.ElapsedMilliseconds;
        }


        [HttpPost]
        public long IsReady()
        {


            return preprocessTime;
        }
    }
}