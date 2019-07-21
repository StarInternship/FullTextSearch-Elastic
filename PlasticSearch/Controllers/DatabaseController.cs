
using Elasticsearch.Net;
using FastMember;
using Nest;
using PlasticSearch.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private LinkedList<Text> files = new LinkedList<Text>();
        ElasticClient client;

        public void Connect()
        {

        }

        public void AddToSendingFiles(string fileName, string text)
        {

        }

        public void sendFiles()
        {

        }

        public List<string> search(string text)
        {
            return null;
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

    public class Table
    {
        public static readonly Table EXACT = new Table("dbo.Exact");
        public static readonly Table NGRAM = new Table("dbo.Ngram");

        private readonly string tableName;
        private Table(string tableName)
        {
            this.tableName = tableName;
        }

        public override string ToString()
        {
            return tableName;
        }

        public static List<Table> Values()
        {
            return new List<Table>()
            {
                EXACT, NGRAM
            };
        }
    }
}