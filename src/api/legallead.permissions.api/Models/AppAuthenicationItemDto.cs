using Newtonsoft.Json;

namespace legallead.permissions.api.Models
{
    public class AppAuthenicationItemDto
    {
        [JsonProperty("id")]
        public int Id { get; set; } = -1;
        [JsonProperty("userName")]
        public string UserName { get; set; } = string.Empty;
        [JsonProperty("secret")]
        public string Code { get; set; } = string.Empty;
    }
}
