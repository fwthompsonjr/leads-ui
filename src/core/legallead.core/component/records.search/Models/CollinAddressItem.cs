using Newtonsoft.Json;

namespace legallead.records.search.Models
{
    internal class CollinAddressItem
    {
        [JsonProperty("caseno")]
        public string CaseNumber { get; set; } = string.Empty;
        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;
        [JsonProperty("defendant")]
        public string Defendant { get; set; } = string.Empty;

        public string Address1 { get; private set; } = string.Empty;
        public string Address2 { get; private set; } = string.Empty;
        public string Address3 { get; private set; } = string.Empty;
        public string Zip { get; private set; } = string.Empty;
        

        public void Parse()
        {
            const char pipe = '|';
            const char space = ' ';
            if (string.IsNullOrEmpty(Address)) return;
            if (!Address.Contains(pipe)) return;
            var parts = Address.Split(pipe);
            var last = parts[^1];
            if (last.Contains(space))
            {
                Zip = last.Split(space)[^1];
            }
            Address1 = parts[0];
            Address3 = last;
            if (parts.Length == 2) return;
            var middle = "";
            for (var m = 1; m < parts.Length - 2; m++)
            {
                if (middle.Length > 0) middle += ";";
                middle += parts[m];
            }
            Address2 = middle;
        }
    }
}
