using Newtonsoft.Json;

namespace legallead.permissions.api.Model
{
    public class UserSearchCounty
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
