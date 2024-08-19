using Newtonsoft.Json;

namespace legallead.permissions.api.Entities
{
    public class QueueInitializeRequestItem
    {
        [JsonProperty("Id")]
        public string Id { get; set; } = string.Empty;
    }
}
