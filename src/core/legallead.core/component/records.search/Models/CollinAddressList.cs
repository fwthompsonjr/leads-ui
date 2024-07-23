using Newtonsoft.Json;

namespace legallead.records.search.Models
{
    internal static class CollinAddressList
    {
        public static List<CollinAddressItem> List { get; set; } = new();
        public static bool Append(string json)
        {
            var item = TryParse<CollinAddressItem>(json);
            if (item == null || string.IsNullOrEmpty(item.CaseNumber)) { return false; }
            item.Parse();
            var found = List.Find(x => x.CaseNumber == item.CaseNumber);
            if (found == null) { List.Add(item); return true; }
            found.Address = item.Address;
            found.Parse();
            return true;
        }

        private static T? TryParse<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
