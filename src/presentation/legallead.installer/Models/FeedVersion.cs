using Newtonsoft.Json;

namespace legallead.installer.Models
{
    public class FeedVersion
    {
        [JsonProperty("@id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("version")]
        public string Version { get; set; } = string.Empty;

        [JsonProperty("downloads")]
        public int Downloads { get; set; }
    }
}
