using Newtonsoft.Json;
using System;

namespace legallead.desktop.models
{
    public class SubscriptionChangeResponseDto
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("externalId")]
        public string? ExternalId { get; set; }

        [JsonProperty("invoiceUri")]
        public string? InvoiceUri { get; set; }

        [JsonProperty("levelName")]
        public string? LevelName { get; set; }

        [JsonProperty("sessionId")]
        public string? SessionId { get; set; }

        [JsonProperty("isPaymentSuccess")]
        public bool IsPaymentSuccess { get; set; }

        [JsonProperty("completionDate")]
        public DateTime? CompletionDate { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
    }

    public class SubscriptionChangeResponse
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("request")]
        public string? Request { get; set; }

        [JsonProperty("dto")]
        public SubscriptionChangeResponseDto? Dto { get; set; }
    }
}
