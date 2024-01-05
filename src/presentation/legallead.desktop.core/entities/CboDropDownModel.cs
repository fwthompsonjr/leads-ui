using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class CboDropDownModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("options")]
        public List<DropDownModel> Members { get; set; } = new();
    }
}