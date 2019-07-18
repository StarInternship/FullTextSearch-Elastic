using PlasticSearch.Models.search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasticSearch.Models.tokenizer
{
    abstract class Tokenizer
    {
        protected static readonly string SPLITTER = "[|\\s|.|'|:|_|-|?|/|@|<|>|!|\\(|\\)|]";

        public string CleanText(string text) => text.ToLower();

        public abstract void TokenizeData(string filePath, string text);

        public abstract ISet<string> TokenizeQuery(string text);

        public abstract List<string> Develope(string token);
    }
}