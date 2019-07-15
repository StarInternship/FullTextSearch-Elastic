using PlasticSearch.Models.search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PlasticSearch.Models.tokenizer
{
    class ExactSearchTokenizer : Tokenizer
    {
        public override ISet<string> TokenizeQuery(string text) => Regex.Split(text, SPLITTER).ToHashSet();

        public override List<string> Develope(string token) => new string[] { token }.ToList();

        public override void tokenizeData(string filePath, string text, IDictionary<string, InvertedIndex> data)
        {
            Regex.Split(text, SPLITTER).ToList().ForEach(token =>
            {
                if (data.ContainsKey(token))
                {
                    if (data[token].ContainsKey(filePath))
                    {
                        data[token][filePath]++;
                    }
                    else
                    {
                        data[token][filePath] = 1;
                    }
                }
                else
                {
                    data[token] = new InvertedIndex
                    {
                        [filePath] = 1
                    };
                }
            });
        }
    }
}