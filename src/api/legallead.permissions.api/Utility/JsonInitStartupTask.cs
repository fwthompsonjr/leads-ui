using legallead.json.db.entity;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api
{
    public class JsonInitStartupTask : IStartupTask
    {
        private bool IsDataReady = false;
        public int Index => 5;

        public JsonInitStartupTask()
        {
        }

        public async Task Execute()
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