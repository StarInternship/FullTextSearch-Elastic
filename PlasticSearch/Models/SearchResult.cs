using System.Collections.Generic;

namespace PlasticSearch.Models
{
    public class SearchResult
    {
        public IEnumerable<Text> Result { get; }
        public long Time { get; }

        public SearchResult(IEnumerable<Text> result, long time)
        {
            Result = result;
            Time = time;
        }
    }
}