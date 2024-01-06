using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class CountySearchDetail
    {
        [JsonProperty("dropDowns")]
        public CboDropDownModel[] DropDowns { get; set; } = Array.Empty<CboDropDownModel>();

        [JsonProperty("caseSearchTypes")]
        public CaseSearchModel[] CaseSearchTypes { get; set; } = Array.Empty<CaseSearchModel>();
    }
}