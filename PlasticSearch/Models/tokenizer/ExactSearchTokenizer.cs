using PlasticSearch.Models.search;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlasticSearch.Models.tokenizer
{
    class ExactSearchTokenizer : Tokenizer
    {
        public override ISet<string> TokenizeQuery(string text) => Regex.Split(text, SPLITTER).ToHashSet();

        public override List<string> Develope(string token) => new string[] { token }.ToList();

        public override void TokenizeData(string filePath, string text, IDictionary<string, InvertedIndex> data)
        {
            Regex.Split(text, SPLITTER).ToList().ForEach(token =>
            {
                if (data.ContainsKey(token))
                {
                    data[token].Add(filePath);
                }
                else
                {
                    data[token] = new InvertedIndex
                    {
                        filePath
                    };
                }
            });
        }
    }
}