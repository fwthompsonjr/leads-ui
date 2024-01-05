using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    internal class CountySearchDetail
    {
        [JsonProperty("dropDowns")]
        public List<CboDropDownModel> DropDowns { get; set; } = new();

        [JsonProperty("caseSearchTypes")]
        public List<CaseSearchModel>? CaseSearchTypes { get; set; }
    }
}