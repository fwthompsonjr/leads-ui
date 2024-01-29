using legallead.jdbc.interfaces;
using MySqlConnector;
using System.Data;

namespace legallead.jdbc.helpers
{
    public class DataContext
    {
        private readonly IDapperCommand _dbexecutor;
        private readonly IDataInitializer _dbinit;

        public DataContext(IDapperCommand command) : this(command, null, local, app)
        {
        }

        internal DataContext(IDapperCommand command, IDataInitializer? dbint, string environment = local, string database = app)
        {
            dbint ??= new DataInitializer();
            _dbinit = dbint;
            EnvironmentName = environment;
            DatabaseName = database;
            _dbexecutor = command;
        }
        public string EnvironmentName { get; private set; }
        public string DatabaseName { get; private set; }

        public virtual IDapperCommand GetCommand => _dbexecutor;

        public virtual IDbConnection CreateConnection()
        {
            var _connectionString = AwsData.GetPostGreString(EnvironmentName, DatabaseName);
            return new MySqlConnection(_connectionString);
        }

        public async Task Init()
        {
            await _dbinit.Init();
        }
        private const string app = "app";
        private const string local = "Test";
    }
}