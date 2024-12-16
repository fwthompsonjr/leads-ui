using Newtonsoft.Json;

namespace legallead.jdbc.models
{
    public class UserUsageAppendRecordModel
    {
        [JsonProperty("lidx")] public string LeadUserId { get; set; } = string.Empty;
        [JsonProperty("cid")] public int CountyId { get; set; }
        [JsonProperty("ctname")] public string CountyName { get; set; } = string.Empty;
        [JsonIgnore] public DateTime StartDate { get; set; }
        [JsonIgnore] public DateTime EndDate { get; set; }
        [JsonProperty("rc")] public int RecordCount { get; set; }

        [JsonProperty("drange")]
        public string DateRange => $"{StartDate:M/d} to {EndDate:M/d}";
        [JsonProperty("sdte")] public string StartDateNode => $"{StartDate:yyyy-MM-dd}";
        [JsonProperty("edte")] public string EndDateNode => $"{EndDate:yyyy-MM-dd}";


    }
}
