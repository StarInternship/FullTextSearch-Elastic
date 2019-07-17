
using FastMember;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();

        private SqlConnection connection;
        private List<Record> ngramTokens;
        private List<Record> exactTokens;
        private DatabaseController()
        {
        }

        internal void Connect()
        {
            ngramTokens = new List<Record> { };
            exactTokens = new List<Record> { };
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
            writeToDB(exactTokens, Table.EXACT);
            writeToDB(ngramTokens, Table.NGRAM);
        }
        private void writeToDB(List<Record> tokens, Table table)
        {
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
                if (exactTokens.Count > 10000)
                    writeToDB(exactTokens, Table.EXACT);
            }
            else if (table.Equals(Table.NGRAM))
            {
                ngramTokens.Add(new Record(dataToken, fileName));
                if (ngramTokens.Count > 10000)
                    writeToDB(ngramTokens, Table.NGRAM);
            }
        }

        public ISet<string> FindFiles(List<string> tokens, Table table)
        {
            string commandString = GenerateSelectCommand(tokens, table);
            SqlCommand command = new SqlCommand(commandString, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            ISet<string> result = new HashSet<string>();
            while (dataReader.Read())
            {
                result.Add(dataReader["file_name"].ToString());
            }
            dataReader.Close();
            command.Dispose();

            return result;
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
        public string token { get; } = "";
        public string file_name { get; } = "";
        public Record(string token, string file_name)
        {
            this.token = token;
            this.file_name = file_name;
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