using legallead.jdbc.interfaces;

namespace legallead.permissions.api
{
    [ExcludeFromCodeCoverage(Justification = "Class directly interacts with database.")]
    public class JdbcInitStartUpTask(IDataInitializer dataDb) : IStartupTask
    {
        private readonly IDataInitializer _dataDb = dataDb;
        public int Index => 10;

        public async Task Execute()
        {
            await _dataDb.Init();
        }
    }
}