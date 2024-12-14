using Newtonsoft.Json;

namespace legallead.jdbc.models
{
    public class UserUsageAppendRecordModel
    {
        [JsonProperty("lidx")] public string LeadUserId { get; set; } = string.Empty;
        [JsonProperty("cid")] public int CountyId { get; set; }
        [JsonProperty("ctname")] public string CountyName { get; set; } = string.Empty;
        [JsonProperty("sdte")] public DateTime StartDate { get; set; }
        [JsonProperty("edte")] public DateTime EndDate { get; set; }
        [JsonProperty("rc")] public int RecordCount { get; set; }

        [JsonProperty("drange")]
        public string DateRange => $"{StartDate:M/d} to {EndDate:M/d}";
    }
}
