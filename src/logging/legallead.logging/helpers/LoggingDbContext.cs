using legallead.logging.interfaces;
using MySqlConnector;
using System.Data;

namespace legallead.logging.helpers
{
    public class LoggingDbContext : ILoggingDbContext
    {
        private readonly string _connectionString;
        private readonly ILoggingDbCommand _dbexecutor;

        public LoggingDbContext(ILoggingDbCommand command)
        {
            _connectionString = LoggingDb.GetConnectionString();
            _dbexecutor = command;
        }

        public virtual ILoggingDbCommand GetCommand => _dbexecutor;

        public virtual IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}