using System.Text.Json.Serialization;

namespace legallead.permissions.api.Models
{
    public class PaymentStripeOption
    {
        public const string Position = "Payment";
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("codes")]
        public PaymentCode Codes { get; set; } = new();
    }
}
