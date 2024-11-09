using Newtonsoft.Json;

namespace legallead.permissions.api.Models
{
    public class AppAuthenicationListDto
    {
        [JsonProperty("keyCode")]
        public string Code { get; set; } = string.Empty;
        public string Vector { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
