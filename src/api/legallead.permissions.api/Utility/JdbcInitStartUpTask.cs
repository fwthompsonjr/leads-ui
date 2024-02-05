using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api
{
    public class JdbcInitStartUpTask : IStartupTask
    {
        private readonly IDataInitializer _dataDb;
        public int Index => 10;

        public JdbcInitStartUpTask(IDataInitializer dataDb)
        { _dataDb = dataDb; }

        public async Task Execute()
        {
            await _dataDb.Init();
        }
    }
}