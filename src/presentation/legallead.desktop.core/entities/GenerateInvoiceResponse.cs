﻿using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    public class GenerateInvoiceResponse
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("paymentIntentId")]
        public string? PaymentIntentId { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("clientSecret")]
        public string? ClientSecret { get; set; }

        [JsonProperty("externalId")]
        public string? ExternalId { get; set; }

        [JsonProperty("data")]
        public List<InvoiceResponseData>? Data { get; set; }
    }
}