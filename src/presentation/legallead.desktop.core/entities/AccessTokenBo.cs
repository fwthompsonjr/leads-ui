using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class AccessTokenBo
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }
    }
}