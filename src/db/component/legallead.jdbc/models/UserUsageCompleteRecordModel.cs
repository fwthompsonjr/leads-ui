using Newtonsoft.Json;

namespace legallead.jdbc.models
{
    public class UserUsageCompleteRecordModel
    {
        [JsonProperty("idx")] public string UsageRecordId { get; set; } = string.Empty;
        [JsonProperty("rc")] public int RecordCount { get; set; }
    }
}
