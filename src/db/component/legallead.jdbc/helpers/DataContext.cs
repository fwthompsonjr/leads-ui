using legallead.jdbc.interfaces;
using MySqlConnector;
using System.Data;

namespace legallead.jdbc.helpers
{
    public class DataContext
    {
        private readonly string _connectionString;
        private readonly IDapperCommand _dbexecutor;
        private readonly IDataInitializer _dbinit;

        public DataContext(IDapperCommand command) : this(command, null)
        {
        }

        internal DataContext(IDapperCommand command, IDataInitializer? dbint)
        {
            dbint ??= new DataInitializer();
            _dbinit = dbint;
            _connectionString = AwsData.GetPostGreString();
            _dbexecutor = command;
        }

        public virtual IDapperCommand GetCommand => _dbexecutor;

        public virtual IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task Init()
        {
            await _dbinit.Init();
        }
    }
}