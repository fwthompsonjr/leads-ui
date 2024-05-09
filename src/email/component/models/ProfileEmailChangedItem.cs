using legallead.email.interfaces;
using Newtonsoft.Json;

namespace legallead.email.models
{
    internal class ProfileEmailChangedItem : IProfileChangeItem
    {
        [JsonProperty("EmailType")]
        public string FieldName { get; set; } = string.Empty;
        [JsonProperty("Email")]
        public string Description { get; set; } = string.Empty;
    }
}