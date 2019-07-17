
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

        private DatabaseController()
        {
        }

        internal void Connect()
        {
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

        public void AddDataToken(string dataToken, string fileName, string tableName)
        {
            SqlCommand command;
            SqlDataReader dataReader;



            command = new SqlCommand("INSERT INTO " + tableName + " " +
        "(token, file_name) " +
                "VALUES(@token, @file_name)",
        connection);

            command.Parameters.Add("@token", System.Data.SqlDbType.Text);
            command.Parameters.Add("@file_name", System.Data.SqlDbType.Text);


            command.Parameters["@token"].Value = dataToken;
            command.Parameters["@file_name"].Value = fileName;

            dataReader = command.ExecuteReader();
            dataReader.Close();
            command.Dispose();
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