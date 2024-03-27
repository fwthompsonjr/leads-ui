using Newtonsoft.Json;

namespace legallead.installer.Models
{
    internal class CommonSettings
    {
        [JsonProperty("key")]
        public string Key { get; set; } = string.Empty;
        [JsonProperty("packages")]
        public List<string> Packages { get; set; } = [];
    }
}
