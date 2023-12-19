using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class ApiContext
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}