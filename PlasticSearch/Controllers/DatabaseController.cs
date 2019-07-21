using Nest;
using System.Collections.Generic;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private LinkedList<Text> files = new LinkedList<Text>();
        private ElasticClient client;

        public void Connect()
        {

        }

        public void AddFile(string fileName, string text)
        {

        }

        public void InsertFiles()
        {

        }

        public List<string> search(string text)
        {
            return new List<string>();
        }
    }

    class Text
    {
        public string text { get; set; }
        public string fileName { get; set; }

        public override int GetHashCode()
        {
            return (text + " ->" + fileName).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
    }
}