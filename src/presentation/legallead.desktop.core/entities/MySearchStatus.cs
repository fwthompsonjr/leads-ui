using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    public class MySearchStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("line")]
        public string Line { get; set; } = string.Empty;

        [JsonProperty("lineNbr")]
        public int LineNbr { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; } = string.Empty;

        [JsonProperty("createDate")]
        public string CreateDate { get; set; } = string.Empty;
    }
}
