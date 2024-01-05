using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class CaseSearchModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}