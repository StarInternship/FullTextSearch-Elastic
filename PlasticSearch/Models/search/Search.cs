using PlasticSearch.Models.tokenizer;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PlasticSearch.Models.search
{
    class Search
    {
        public void search(List<string> queryTokens, ISet<string> result, Tokenizer tokenizer, Table table)
        {
            List<Task> tasks = new List<Task>();
            bool first = true;
            foreach (string queryToken in queryTokens)
            {
                ISet<string> foundFilePaths = new HashSet<string>();

                Task task = new Task(() =>
                {
                    FindFiles(queryToken, foundFilePaths, tokenizer, table);

                    lock (result)
                    {
                        if (first)
                        {
                            first = false;
                            result.UnionWith(foundFilePaths);
                        }
                        else
                        {
                            result.IntersectWith(foundFilePaths);
                        }
                    }
                });
                tasks.Add(task);
                task.Start();
            }
            Task.WaitAll(tasks.ToArray());
        }

        private void FindFiles(string queryToken, ISet<string> foundFilePaths, Tokenizer queryTokenizer, Table table)
        {
            List<string> developedTokens = queryTokenizer.Develope(queryToken);
            foundFilePaths.UnionWith(DatabaseController.Instance.FindFiles(developedTokens, table));
        }
    }
}