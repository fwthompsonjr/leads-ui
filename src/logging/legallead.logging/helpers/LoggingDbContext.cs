using legallead.logging.interfaces;
using MySqlConnector;
using System;
using System.Data;

namespace legallead.logging.helpers
{
    public class LoggingDbContext : ILoggingDbContext
    {
        private readonly ILoggingDbCommand _dbexecutor;
        private readonly string EnvironmentName;
        private readonly string DatabaseName;

        public LoggingDbContext(ILoggingDbCommand command) : this(command, environ, db)
        {
        }

        public LoggingDbContext(ILoggingDbCommand command, string environment, string database)
        {
            EnvironmentName = environment;
            DatabaseName = database;
            _dbexecutor = command;
        }

        public virtual ILoggingDbCommand GetCommand => _dbexecutor;

        public virtual IDbConnection CreateConnection()
        {
            var connectionString = LoggingDb.GetConnectionString(EnvironmentName, DatabaseName);
            return new MySqlConnection(connectionString);
        }
        private const string environ = "test";
        private const string db = "error";
    }
}