using Newtonsoft.Json;

namespace legallead.installer.Models
{
    internal class CommonSettings
    {
        [JsonProperty("key")]
        public string Key { get; set; } = string.Empty;
        [JsonProperty("createShortcut")]
        public bool CreateShortcut { get; set; }
        [JsonProperty("product")]
        public string Product { get; set; } = string.Empty;
        [JsonProperty("repository")]
        public string Repository { get; set; } = string.Empty;
        [JsonProperty("packages")]
        public List<string> Packages { get; set; } = [];
    }
}
