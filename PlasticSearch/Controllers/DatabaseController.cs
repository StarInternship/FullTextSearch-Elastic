
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PlasticSearch
{
    internal class DatabaseController
    {
        public static DatabaseController Instance { get; } = new DatabaseController();
        private static readonly string connectionString = @"Data Source=.;Initial Catalog=PlasticSearch;User ID=sa;Password=123456;Integrated Security=True;";
        private readonly SqlConnection connection = new SqlConnection(connectionString);

        private DatabaseController()
        {
        }

        internal void Connect()
        {
            connection.Open();
        }

        public ISet<string> FindFiles(List<string> tokens, string tableName)
        {
            return new HashSet<string>();
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
    }
}