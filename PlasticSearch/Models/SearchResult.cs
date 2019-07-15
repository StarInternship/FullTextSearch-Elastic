using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasticSearch.Models
{
    public class SearchResult
    {
        public ISet<string> Result { get; }

        public SearchResult(ISet<string> result)
        {
            Result = result;
        }
    }
}