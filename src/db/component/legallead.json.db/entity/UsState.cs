using Newtonsoft.Json;
using System.Text;

namespace legallead.json.db.entity
{
    public class UsState
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public bool IsActive { get; set; }

        public static void Initialize()
        {
            var list = JsonConvert.DeserializeObject<List<UsState>>(GetList) ?? new();
            UsStatesList.Populate(list);
        }

        private static string? _list;
        private static string GetList => _list ??= GetJsonList();

        private static string GetJsonList()
        {
            return Properties.Resources.state_list_json;
        }
    }
}