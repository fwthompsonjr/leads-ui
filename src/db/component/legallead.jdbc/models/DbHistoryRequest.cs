using Newtonsoft.Json;

namespace legallead.jdbc.models
{
    public class DbHistoryRequest
    {
        [JsonProperty("cid")] public int CountyId { get; set; }
        [JsonProperty("dte")] public DateTime SearchDate { get; set; }
        [JsonProperty("stid")] public int SearchTypeId { get; set; }
        [JsonProperty("ctid")] public int CaseTypeId { get; set; }
        [JsonProperty("dcid")] public int DistrictCourtId { get; set; }
        [JsonProperty("dsid")] public int DistrictSearchTypeId { get; set; }
    }
}
