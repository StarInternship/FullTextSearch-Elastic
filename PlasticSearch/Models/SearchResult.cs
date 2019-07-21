using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasticSearch.Models
{
    public class SearchResult
    {
        public ISet<string> result { get; }
        public long time { get; }

        public SearchResult(ISet<string> result, long time)
        {
            this.result = result;
            this.time = time;
        }
    }
}