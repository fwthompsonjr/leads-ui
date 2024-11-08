using Newtonsoft.Json;

namespace legallead.permissions.api.Models
{
    public class AuthorizedCountyModel
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        [JsonProperty("user")]
        public string UserId { get; set; } = string.Empty;
    }
}
