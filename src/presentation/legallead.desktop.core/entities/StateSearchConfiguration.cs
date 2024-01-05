using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class StateSearchConfiguration
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("shortName")]
        public string? ShortName { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        public List<CountySearchConfiguration>? Counties { get; set; }
    }
}