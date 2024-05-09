using Newtonsoft.Json;

namespace legallead.email.models
{
    internal class DbSetting
    {
        [JsonProperty("server")]
        public string Server { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string DataBase { get; set; } = string.Empty;

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("key")]
        public string Key { get; set; } = string.Empty;
    }
}
