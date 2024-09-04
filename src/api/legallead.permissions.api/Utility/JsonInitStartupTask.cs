using legallead.json.db.entity;

namespace legallead.permissions.api
{
    [ExcludeFromCodeCoverage(Justification = "Class directly interacts with database.")]
    public class JsonInitStartupTask : IStartupTask
    {
        private bool IsDataReady = false;
        public int Index => 5;

        public JsonInitStartupTask()
        {
        }

        public async Task ExecuteAsync()
        {
            if (IsDataReady) { return; }
            await Task.Run(() =>
            {
                UsStateCounty.Initialize();
                UsState.Initialize();
            });
            IsDataReady = true;
        }
    }
}