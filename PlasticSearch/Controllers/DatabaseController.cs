
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

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
            string commandString = GenerateSelectCommand(tokens, tableName);
            SqlCommand command = new SqlCommand(commandString, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            ISet<string> result = new HashSet<string>();
            while (dataReader.Read())
            {
                result.Add(dataReader["file_name"].ToString());
            }
            dataReader.Close();
            command.Dispose();

            return new HashSet<string>();
        }

        private static string GenerateSelectCommand(List<string> tokens, string tableName)
        {
            StringBuilder commandString = new StringBuilder(
                "SELECT file_name FROM " + tableName + " WHERE token IN ("
                );
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                commandString.Append("'" + tokens[i] + "', ");
            }
            commandString.Append("'" + tokens[tokens.Count - 1] + "');");
            return commandString.ToString();
        }
    }
}