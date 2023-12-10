using legallead.json.db.entity;
using legallead.json.db.interfaces;

namespace legallead.json.db
{
    public class JsonDataInitializer : IJsonDataInitializer
    {
        private bool IsDbInitialized = false;

        public JsonDataInitializer()
        {
        }

        public async Task InitTables()
        {
            if (IsDbInitialized) return;
            await Task.Run(() =>
            {
                UsState.Initialize();
                UsStateCounty.Initialize();
            });
            IsDbInitialized = true;
        }
    }
}