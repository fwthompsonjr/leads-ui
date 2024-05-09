using legallead.email.interfaces;
using Newtonsoft.Json;

namespace legallead.email.models
{
    internal class ProfileNameChangedItem : IProfileChangeItem
    {
        [JsonProperty("NameType")]
        public string FieldName { get; set; } = string.Empty;
        [JsonProperty("Name")]
        public string Description { get; set; } = string.Empty;
    }
}