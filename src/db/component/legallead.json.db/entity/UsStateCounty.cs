using Newtonsoft.Json;

namespace legallead.json.db.entity
{
    public class UsStateCounty
    {
        private static readonly object locker = new();
        public string? Name { get; set; }
        public int Index { get; set; }
        public string? StateCode { get; set; }
        public string? ShortName { get; set; }
        public bool IsActive { get; set; }

        public static void Initialize()
        {
            lock (locker)
            {
                if (IsInitialized) return;
                var tmp = GetList;
                var list = JsonConvert.DeserializeObject<List<UsStateCounty>>(tmp) ?? new();
                UsStateCountyList.Populate(list);
                IsInitialized = true;
            }
        }

        private static bool IsInitialized;
        private static string? _list;

        private static string GetList
        {
            get
            {
                if (!string.IsNullOrEmpty(_list)) return _list;
                _list = GetJsonList();
                return _list;
            }
        }

        private static string GetJsonList()
        {
            return Properties.Resources.county_list_json;
        }
    }
}