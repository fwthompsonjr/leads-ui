using legallead.json.db.entity;
using legallead.json.db.interfaces;

namespace legallead.json.db
{
    public class JsonDataInitializer : IJsonDataInitializer
    {
        private readonly IJsonDataProvider _provider;
        private bool IsDbInitialized = false;

        public JsonDataInitializer(IJsonDataProvider provider)
        {
            _provider = provider;
        }

        public async Task InitTables()
        {
            if (IsDbInitialized) return;
            await Task.Run(() =>
            {
                UsState.Initialize(_provider);
                UsStateCounty.Initialize(_provider);
            });
            IsDbInitialized = true;
        }
    }
}