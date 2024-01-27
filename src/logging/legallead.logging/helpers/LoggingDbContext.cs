using legallead.logging.interfaces;
using MySqlConnector;
using System.Data;

namespace legallead.logging.helpers
{
    public class LoggingDbContext : ILoggingDbContext
    {
        private readonly string _connectionString;
        private readonly ILoggingDbCommand _dbexecutor;

        public LoggingDbContext(ILoggingDbCommand command) : this(command, environ, db)
        {
        }

        public LoggingDbContext(ILoggingDbCommand command, string environment, string database)
        {
            _connectionString = LoggingDb.GetConnectionString(environment, database);
            _dbexecutor = command;
        }

        public virtual ILoggingDbCommand GetCommand => _dbexecutor;

        public virtual IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
        private const string environ = "local";
        private const string db = "error";
    }
}