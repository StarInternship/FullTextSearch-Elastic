
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
    }
}