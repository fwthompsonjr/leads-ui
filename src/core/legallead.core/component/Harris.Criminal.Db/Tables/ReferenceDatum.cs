using Newtonsoft.Json;

namespace Harris.Criminal.Db.Tables
{
    public class ReferenceDatum
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("literal")]
        public string Literal { get; set; } = string.Empty;
    }
}