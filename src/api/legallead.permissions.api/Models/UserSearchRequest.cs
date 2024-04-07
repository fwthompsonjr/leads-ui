using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace legallead.permissions.api.Model
{
    public class UserSearchRequest
    {
        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;

        [JsonProperty("county")]
        public UserSearchCounty County { get; set; } = new();

        [JsonProperty("details")]
        public List<CountyParameterModel> Details { get; set; } = new();

        [JsonProperty("start")]
        [JsonPropertyName("start")]
        public long StartDate { get; set; }

        [JsonProperty("end")]
        [JsonPropertyName("end")]
        public long EndDate { get; set; }
    }
}
