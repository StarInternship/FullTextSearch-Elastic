using PlasticSearch.Models.search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PlasticSearch.Models.tokenizer
{
    class NgramSearchTokenizer : Tokenizer
    {
        private static readonly int MIN = 3;
        private static readonly int MAX = 30;

        public override ISet<string> TokenizeQuery(string text)
        {
            ISet<string> tokenSet = new HashSet<string>();

            Regex.Split(text, SPLITTER).ToList().ForEach(token =>
            {
                if (token.Length > MIN)
                {
                    int max = Math.Min(MAX, token.Length);

                    for (int length = MIN; length <= MAX; length++)
                    {
                        for (int start = 0; start + length < +token.Length; start++)
                        {
                            tokenSet.Add(token.Substring(start, length));
                        }
                    }
                }
            });

            return tokenSet;
        }
        public override List<string> Develope(string token) => (new string[] { token }).ToList();

        public override void tokenizeData(string filePath, string text, IDictionary<string, InvertedIndex> data)
        {
            Regex.Split(text, SPLITTER).ToList().ForEach(token =>
            {
                if (token.Length > MIN)
                {
                    int max = Math.Min(MAX, token.Length);

                    for (int length = MIN; length <= MAX; length++)
                    {
                        for (int start = 0; start + length < token.Length; start++)
                        {
                            string newToken = token.Substring(start, length);

                            if (data.ContainsKey(newToken))
                            {
                                if (data[newToken].ContainsKey(filePath))
                                {
                                    data[newToken][filePath]++;
                                }
                                else
                                {
                                    data[newToken][filePath] = 1;
                                }
                            }
                            else
                            {
                                data[newToken] = new InvertedIndex
                                {
                                    [filePath] = 1
                                };
                            }
                        }
                    }
                }
            });
        }
    }
}