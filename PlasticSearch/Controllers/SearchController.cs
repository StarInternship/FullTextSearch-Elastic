using PlasticSearch.Models;
using PlasticSearch.Models.search;
using PlasticSearch.Models.tokenizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PlasticSearch.Controllers
{
    public class SearchController
    {
        public static SearchController Instance { get; } = new SearchController();
        private readonly Tokenizer exactSearchTokenizer = new ExactSearchTokenizer();
        private readonly Tokenizer ngramSearchTokenizer = new NgramSearchTokenizer();
        private readonly Search search = new Search();
        private ISet<string> result;
        private ISet<string> queryTokens;
        private readonly Stopwatch sw = new Stopwatch();
        private long preprocessTime = -1;
        private Thread preprocessThread;

        private SearchController()
        {
        }

        public void Preprocess()
        {
            preprocessThread = new Thread(() =>
            {
                sw.Start();

                Importer importer = new Importer();
                sw.Start();

                importer.ReadFiles();


                sw.Stop();
                preprocessTime = sw.ElapsedMilliseconds;
            });
            preprocessThread.Start();
        }


        internal void addFile(string path, string text)
        {
            
                string cleanText = ngramSearchTokenizer.CleanText(text);
                ngramSearchTokenizer.TokenizeData(path, cleanText);
                exactSearchTokenizer.TokenizeData(path, cleanText);
        }

        public SearchResult Search(string query)
        {
            result = new HashSet<string>();

            QueryProcess(query);
            long searchTime = DoSearch();

            return new SearchResult(result, searchTime);
        }

        void QueryProcess(string query)
        {
            sw.Restart();

            queryTokens = exactSearchTokenizer.TokenizeQuery(ngramSearchTokenizer.CleanText(query));

        }

        long DoSearch()
        {

            search.search(queryTokens.ToList(), result);

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public long GetIsReady()
        {
            preprocessThread.Join();

            return preprocessTime;
        }
    }
}