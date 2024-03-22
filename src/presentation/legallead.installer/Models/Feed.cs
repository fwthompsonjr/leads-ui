using Newtonsoft.Json;

namespace legallead.installer.Models
{
    public class Feed
    {
        [JsonProperty("@context")]
        public FeedContext Context { get; set; } = new();

        [JsonProperty("totalHits")]
        public int TotalHits { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; } = string.Empty;

        [JsonProperty("lastReopen")]
        public DateTime LastReopen { get; set; }

        [JsonProperty("data")]
        public List<FeedDatum> Data { get; set; } = [];
    }
}
