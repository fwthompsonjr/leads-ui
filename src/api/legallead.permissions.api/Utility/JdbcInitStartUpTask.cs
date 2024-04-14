using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api
{
    [ExcludeFromCodeCoverage(Justification = "Class directly interacts with database.")]
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