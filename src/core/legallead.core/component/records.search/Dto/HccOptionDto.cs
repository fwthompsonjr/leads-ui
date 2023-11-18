using Newtonsoft.Json;

namespace legallead.records.search.Dto
{
    public class HccOptionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; } = string.Empty;

        [JsonProperty("type")] public string Type { get; set; } = string.Empty;

        [JsonProperty("labels")]
        public List<string> Labels { get; set; } = new();

        [JsonProperty("index")]
        public int? Index { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("values")]
        public List<string> Values { get; set; } = new();
    }
}