using System.Collections.Generic;

namespace PlasticSearch.Models
{
    public class SearchResult
    {
        public IEnumerable<string> Result { get; }
        public long Time { get; }

        public SearchResult(IEnumerable<string> result, long time)
        {
            Result = result;
            Time = time;
        }
    }
}