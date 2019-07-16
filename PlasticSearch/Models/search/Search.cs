using PlasticSearch.Models.tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasticSearch.Models.search
{
    class Search
    {
        public IDictionary<string, InvertedIndex> ExactData { get; } = new Dictionary<string, InvertedIndex>();
        public IDictionary<string, InvertedIndex> NgramData { get; } = new Dictionary<string, InvertedIndex>();
        private readonly Tokenizer exactSearchTokenizer = new ExactSearchTokenizer();
        private readonly Tokenizer ngramSearchTokenizer = new NgramSearchTokenizer();
        private readonly Tokenizer fuzzySearchTokenizer = new FuzzySearchTokenizer();

        public void search(List<string> queryTokens, ISet<string> result)
        {
            bool first = true;
            foreach (string queryToken in queryTokens)
            {
                ISet<string> foundFilePaths = new HashSet<string>();

                FindFiles(queryToken, foundFilePaths, exactSearchTokenizer, ExactData);
                FindFiles(queryToken, foundFilePaths, ngramSearchTokenizer, NgramData);
                FindFiles(queryToken, foundFilePaths, fuzzySearchTokenizer, ExactData);

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

        private void FindFiles(string queryToken, ISet<string> foundFilePaths, Tokenizer queryTokenizer, IDictionary<string, InvertedIndex> data)
        {
            List<string> developedTokens = queryTokenizer.Develope(queryToken);

            developedTokens.ForEach(token =>
            {
                if (data.ContainsKey(token))
                {
                    foundFilePaths.UnionWith(data[token]);
                }
            });
        }
    }
}