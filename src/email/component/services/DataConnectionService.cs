using MySqlConnector;
using System.Data;

namespace legallead.email.services
{
    internal class DataConnectionService(IConnectionStringService connection)
    {
        private readonly IConnectionStringService connService = connection;

        public virtual IDbConnection CreateConnection()
        {
            var _connectionString = connService.ConnectionString();
            return new MySqlConnection(_connectionString);
        }
    }
}
