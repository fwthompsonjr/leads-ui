using Newtonsoft.Json;

namespace page.load.utility.Entities
{
    public class CaseItemDto
    {
        [JsonProperty("uri")]
        public string Href { get; set; } = string.Empty;
        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; } = string.Empty;
        [JsonProperty("filedate")]
        public string FileDate { get; set; } = string.Empty;
        [JsonProperty("casetype")]
        public string CaseType { get; set; } = string.Empty;
        [JsonProperty("casestatus")]
        public string CaseStatus { get; set; } = string.Empty;
        [JsonProperty("location")]
        public string Court { get; set; } = string.Empty;
        [JsonProperty("partyname")]
        public string PartyName { get; set; } = string.Empty;
        public string Plaintiff { get; set; } = string.Empty;
        [JsonProperty("caseStyle")]
        public string CaseStyle { get; set; } = string.Empty;
        [JsonProperty("courtDate")]
        public string CourtDate { get; set; } = string.Empty;
        [JsonProperty("hearingType")]
        public string HearingType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
