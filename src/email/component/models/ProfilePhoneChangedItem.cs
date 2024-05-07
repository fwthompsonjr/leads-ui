using legallead.email.interfaces;
using Newtonsoft.Json;

namespace legallead.email.models
{
    internal class ProfilePhoneChangedItem : IProfileChangeItem
    {
        [JsonProperty("PhoneType")]
        public string FieldName { get; set; } = string.Empty;
        [JsonProperty("Phone")]
        public string Description { get; set; } = string.Empty;
    }
}
