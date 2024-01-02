using Newtonsoft.Json;

namespace legallead.json.db.entity
{
    public class UsState
    {
        private static readonly object locker = new();
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public bool IsActive { get; set; }

        public static void Initialize()
        {
            lock (locker)
            {
                if (IsInitialized) return;
                var list = JsonConvert.DeserializeObject<List<UsState>>(GetList) ?? new();
                UsStatesList.Populate(list);
                IsInitialized = true;
            }
        }

        private static bool IsInitialized;
        private static string? _list;
        private static string GetList => _list ??= GetJsonList();

        private static string GetJsonList()
        {
            return Properties.Resources.state_list_json;
        }
    }
}