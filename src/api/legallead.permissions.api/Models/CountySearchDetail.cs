using Newtonsoft.Json;

namespace legallead.permissions.api.Model
{
    public class CountySearchDetail
    {
        [JsonProperty("dropDowns")]
        public List<CboDropDown> DropDowns { get; set; } = new();

        [JsonProperty("caseSearchTypes")]
        public List<CaseSearchType>? CaseSearchTypes { get; set; }
    }
}