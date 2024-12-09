using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class DbSearchHistoryResultBo
    {
        public string Id { get; set; } = string.Empty;
        [JsonProperty("name")] public string Name { get; set; } = string.Empty;
        [JsonProperty("zip")] public string Zip { get; set; } = string.Empty;
        [JsonProperty("add1")] public string Address1 { get; set; } = string.Empty;
        [JsonProperty("add2")] public string Address2 { get; set; } = string.Empty;
        [JsonProperty("add3")] public string Address3 { get; set; } = string.Empty;
        [JsonProperty("cnbr")] public string CaseNumber { get; set; } = string.Empty;
        [JsonProperty("fdte")] public string DateFiled { get; set; } = string.Empty;
        [JsonProperty("court")] public string Court { get; set; } = string.Empty;
        [JsonProperty("cstype")] public string CaseType { get; set; } = string.Empty;
        [JsonProperty("cstyle")] public string CaseStyle { get; set; } = string.Empty;
        [JsonProperty("plntf")] public string Plaintiff { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; }
    }
}
