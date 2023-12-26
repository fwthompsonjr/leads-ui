using Newtonsoft.Json;

namespace legallead.desktop.models
{
    internal class LoginFormModel
    {
        [JsonProperty("username")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("login-password")]
        public string Password { get; set; } = string.Empty;
    }
}