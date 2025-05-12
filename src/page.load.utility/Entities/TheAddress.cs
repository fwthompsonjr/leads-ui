using Newtonsoft.Json;

namespace page.load.utility.Entities
{
    public class TheAddress
    {
        [JsonProperty("addr")] public string Address { get; set; } = string.Empty;
        [JsonProperty("plaintiff")] public string Plaintiff { get; set; } = string.Empty;
        [JsonProperty("casenbr")] public string CaseNumber { get; set; } = string.Empty;
        [JsonProperty("caseheader")] public string CaseHeader { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(CaseNumber)) return false;
                if (string.IsNullOrEmpty(CaseHeader)) return false;
                return true;
            }
        }
    }
}
