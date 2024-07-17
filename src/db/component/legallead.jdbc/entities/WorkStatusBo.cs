using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class WorkStatusBo
    {
        [JsonProperty("src")]
        public string Source { get; set; } = Environment.MachineName;
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        [JsonProperty("messageId")]
        public int MessageId { get; set; }
        [JsonProperty("statusId")]
        public int StatusId { get; set; }
    }
}
