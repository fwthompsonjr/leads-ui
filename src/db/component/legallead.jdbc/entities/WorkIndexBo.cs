using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class WorkIndexBo
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
    }
}
