using Newtonsoft.Json;

namespace legallead.desktop.models
{
    internal class PaymentProcessModel
    {
        [JsonProperty("isLive")]
        public bool IsLive { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; } = string.Empty;
    }
}
