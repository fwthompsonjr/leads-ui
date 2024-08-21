using Newtonsoft.Json;

namespace legallead.permissions.api.Entities
{
    public class BaseQueueRequest
    {
        [JsonProperty("Source", NullValueHandling = NullValueHandling.Ignore)]
        public string? Source { get; set; }
    }
}
