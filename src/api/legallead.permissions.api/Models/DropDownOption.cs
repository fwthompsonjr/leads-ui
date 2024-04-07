using Newtonsoft.Json;

namespace legallead.permissions.api.Model
{
    public class DropDownOption
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}