using Newtonsoft.Json;

namespace Harris.Criminal.Db.Tables
{
    public class ReferenceTable
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("data")]
        public List<ReferenceDatum> Data { get; set; } = new();
    }
}