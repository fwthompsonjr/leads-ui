using legallead.email.interfaces;
using Newtonsoft.Json;

namespace legallead.email.models
{

    internal class ProfileAddressChangedItem : IProfileChangeItem
    {
        [JsonProperty("AddressType")]
        public string FieldName { get; set; } = string.Empty;
        [JsonProperty("Address")]
        public string Description { get; set; } = string.Empty;
    }
}