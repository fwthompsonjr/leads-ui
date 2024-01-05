using Newtonsoft.Json;

namespace legallead.permissions.api.Model
{
    public class CboDropDown
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("options")]
        public List<DropDownOption> Members { get; set; } = new();
    }
}