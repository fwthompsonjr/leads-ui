using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class UserSearchFilterBo
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("county")]
        public string County { get; set; } = string.Empty;
        public bool HasFilter
        {
            get
            {
                if (Index != 0) return true;
                if (!string.IsNullOrEmpty(County)) return true;
                return false;
            }
        }
    }
}
