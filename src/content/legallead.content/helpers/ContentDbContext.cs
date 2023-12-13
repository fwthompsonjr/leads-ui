using legallead.content.interfaces;
using MySqlConnector;
using System.Data;

namespace legallead.content.helpers
{
    public class ContentDbContext
    {
        private readonly string _connectionString;
        private readonly IContentDbCommand _dbexecutor;

        public ContentDbContext(IContentDbCommand command)
        {
            _connectionString = ContentDb.GetPostGreString();
            _dbexecutor = command;
        }

        public virtual IContentDbCommand GetCommand => _dbexecutor;

        public virtual IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}