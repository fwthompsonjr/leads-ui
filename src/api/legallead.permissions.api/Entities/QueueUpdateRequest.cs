using Newtonsoft.Json;

namespace legallead.permissions.api.Entities
{
    public class QueueUpdateRequest : BaseQueueRequest
    {
        [JsonProperty("Id")]
        public string? Id { get; set; }
        [JsonProperty("SearchId")]
        public string? SearchId { get; set; }
        [JsonProperty("Message")]
        public string? Message { get; set; }
        [JsonProperty("StatusId")]
        public int? StatusId { get; set; }
    }
}
