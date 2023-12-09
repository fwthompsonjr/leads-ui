using legallead.json.db.entity;
using legallead.json.db.interfaces;

namespace legallead.permissions.api
{
    public class JsonInitStartupTask : IStartupTask
    {
        private bool IsDataReady = false;
        private readonly IJsonDataProvider _dataProvider;
        public int Index => 5;

        public JsonInitStartupTask(IJsonDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task Execute()
        {
            if (IsDataReady) { return; }
            await Task.Run(() =>
            {
                UsStateCounty.Initialize(_dataProvider);
                UsState.Initialize(_dataProvider);
            });
            IsDataReady = true;
        }
    }
}