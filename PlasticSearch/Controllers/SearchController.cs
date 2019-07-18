using PlasticSearch.Models;
using PlasticSearch.Models.search;
using PlasticSearch.Models.tokenizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlasticSearch.Controllers
{
    public class SearchController
    {
        public static SearchController Instance { get; } = new SearchController();
        private readonly Dictionary<string, SearchType> searchType = new Dictionary<string, SearchType>();
        private readonly Search search = new Search();
        private ISet<string> result;
        private ISet<string> queryTokens;
        private readonly Stopwatch sw = new Stopwatch();
        private long preprocessTime = -1;
        private Thread preprocessThread;
        public List<Task> writersToDb { get; } = new List<Task>();
        private SearchController()
        {
            searchType["Exact"] = SearchType.EXACT;
            searchType["Ngram"] = SearchType.NGRAM;
            searchType["Fuzzy"] = SearchType.FUZZY;
        }

        public void Preprocess()
        {
            preprocessThread = new Thread(() =>
            {
                sw.Start();
                Importer.CreateLog();

                Importer.WriteLog("salam");

                Importer importer = new Importer();

                importer.ReadFiles();


                Task.WaitAll(importer.readers.ToArray());

                DatabaseController.Instance.WriteTokensToDatabase();

                Task.WaitAll(writersToDb.ToArray());

                DatabaseController.Instance.CreateIndex();

                sw.Stop();
                preprocessTime = sw.ElapsedMilliseconds;
            });
            preprocessThread.Start();
        }

        internal void AddFile(string path, string text)
        {
                          
            string cleanText = searchType["Exact"].Tokenizer.CleanText(text);
            searchType["Ngram"].Tokenizer.TokenizeData(path, cleanText);
            searchType["Exact"].Tokenizer.TokenizeData(path, cleanText);
        }

        public SearchResult Search(string query, string type)
        {
            result = new HashSet<string>();

            QueryProcess(query);
            long searchTime = DoSearch(type);

            return new SearchResult(result, searchTime);
        }

        void QueryProcess(string query)
        {
            sw.Restart();
            queryTokens = searchType["Exact"].Tokenizer.TokenizeQuery(searchType["Exact"].Tokenizer.CleanText(query));
        }

        long DoSearch(string type)
        {
            search.search(queryTokens.ToList(), result, searchType[type].Tokenizer, searchType[type].Table);

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public long GetIsReady()
        {
            preprocessThread.Join();

            return preprocessTime;
        }
    }

    internal class SearchType
    {
        public static readonly SearchType EXACT = new SearchType(new ExactSearchTokenizer(), Table.EXACT);
        public static readonly SearchType NGRAM = new SearchType(new NgramSearchTokenizer(), Table.NGRAM);
        public static readonly SearchType FUZZY = new SearchType(new FuzzySearchTokenizer(), Table.EXACT);
        internal Tokenizer Tokenizer { get; }
        internal Table Table { get; }

        private SearchType(Tokenizer tokenizer, Table table)
        {
            Tokenizer = tokenizer;
            Table = table;
        }
    }
}