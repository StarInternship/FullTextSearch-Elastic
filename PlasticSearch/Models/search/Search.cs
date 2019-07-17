using PlasticSearch.Models.tokenizer;
using System.Collections.Generic;

namespace PlasticSearch.Models.search
{
    class Search
    {
        public void search(List<string> queryTokens, ISet<string> result, Tokenizer tokenizer, Table table)
        {
            bool first = true;
            foreach (string queryToken in queryTokens)
            {
                ISet<string> foundFilePaths = new HashSet<string>();

                FindFiles(queryToken, foundFilePaths, tokenizer, table);

                if (first)
                {
                    result.UnionWith(foundFilePaths);
                    first = false;
                }
                else
                {
                    result.IntersectWith(foundFilePaths);
                }
            }
        }

        private void FindFiles(string queryToken, ISet<string> foundFilePaths, Tokenizer queryTokenizer, Table table)
        {
            List<string> developedTokens = queryTokenizer.Develope(queryToken);
            foundFilePaths.UnionWith(DatabaseController.Instance.FindFiles(developedTokens, table));
        }
    }
}