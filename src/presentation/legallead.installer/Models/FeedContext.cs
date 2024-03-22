using Newtonsoft.Json;

namespace legallead.installer.Models
{
    public class FeedContext
    {
        [JsonProperty("@vocab")]
        public string Vocab { get; set; } = string.Empty;
    }
}
