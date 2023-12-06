using legallead.jdbc.interfaces;

namespace legallead.permissions.api
{
    public class JdbcInitStartUpTask : IStartupTask
    {
        private readonly IDataInitializer _dataDb;
        public JdbcInitStartUpTask(IDataInitializer dataDb) { _dataDb = dataDb; }
        public async Task Execute()
        {
            await _dataDb.Init();
        }
    }
}
