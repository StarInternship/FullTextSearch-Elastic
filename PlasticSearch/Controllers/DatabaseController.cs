
using FastMember;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private SqlConnection connection;
        private HashSet<Record> ngramTokens;
        private HashSet<Record> exactTokens;
        private DatabaseController()
        {
        }

        internal void Connect()
        {
            ngramTokens = new HashSet<Record> { };
            exactTokens = new HashSet<Record> { };
            string connectionString = @"Data Source=.;Initial Catalog=PlasticSearch;User ID=sa;Password=123456;Integrated Security=True;";
            connection = new SqlConnection(connectionString);
            connection.Open();
            DeletePreviousData();
        }

        private void DeletePreviousData()
        {
            Table.Values().ForEach(table =>
            {
                string commandString = "TRUNCATE TABLE " + table;
                SqlCommand command = new SqlCommand(commandString, connection);
                command.ExecuteReader().Close();
                command.Dispose();
            });
        }

        public void WriteTokensToDatabase()
        {
            WriteToDB(exactTokens, Table.EXACT);
            WriteToDB(ngramTokens, Table.NGRAM);
        }
        private void WriteToDB(HashSet<Record> tokens, Table table)
        {
            //HashSet<Record> data = new HashSet<Record>(tokens);
            //Task writer = new Task(() =>
            //{
            //    using (var bcp = new SqlBulkCopy(connection))
            //    {

            //        using (var reader = ObjectReader.Create(data, "token", "file_name"))
            //        {
            //            bcp.DestinationTableName = table.ToString();
            //            bcp.WriteToServer(reader);
            //        }
            //    }
            //    data.Clear();
            //});

            //writer.Start();
            //tokens.Clear();


            using (var bcp = new SqlBulkCopy(connection))
            {

                using (var reader = ObjectReader.Create(tokens, "token", "file_name"))
                {
                    bcp.DestinationTableName = table.ToString();
                    bcp.WriteToServer(reader);
                }
            }
            tokens.Clear();
        }

        public void AddDataToken(string dataToken, string fileName, Table table)
        {
            if (dataToken == "")
                return;
            if (table.Equals(Table.EXACT))
            {
                exactTokens.Add(new Record(dataToken, fileName));
                if (exactTokens.Count > 100000)
                    WriteToDB(exactTokens, Table.EXACT);
            }
            else if (table.Equals(Table.NGRAM))
            {
                ngramTokens.Add(new Record(dataToken, fileName));
                if (ngramTokens.Count > 100000)
                    WriteToDB(ngramTokens, Table.NGRAM);
            }
        }

        public ISet<string> FindFiles(List<string> tokens, Table table)
        {
            using (var context = new PlasticSearchEntities())
            {
                switch(table.ToString())
                {
                    case "dbo.Exact":
                        return (from index in context.Exacts where tokens.Contains(index.token) select index.file_name).ToHashSet();
                    case "dbo.Ngram":
                        return (from index in context.Ngrams where tokens.Contains(index.token) select index.file_name).ToHashSet();
                    default:
                        return new HashSet<string>();
                }
            }
        }

        private static string GenerateSelectCommand(List<string> tokens, Table table)
        {
            StringBuilder commandString = new StringBuilder("SELECT file_name FROM " + table + " WHERE token IN (");
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                commandString.Append("'" + tokens[i] + "', ");
            }
            commandString.Append("'" + tokens[tokens.Count - 1] + "');");
            return commandString.ToString();
        }
    }

    class Record
    {
        public string token { get; }
        public string file_name { get; }
        public Record(string token, string file_name)
        {
            this.token = token;
            this.file_name = file_name;
        }

        public override int GetHashCode()
        {
            return (token + " ->" + file_name).GetHashCode();
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