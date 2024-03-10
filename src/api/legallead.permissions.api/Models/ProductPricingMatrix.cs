using Newtonsoft.Json;

namespace legallead.permissions.api.Models
{
    public class ProductPricingMatrix
    {
        [JsonProperty("product")]
        public BillingProduct Product { get; set; } = new();

        [JsonProperty("pricecode")]
        public BillingPriceCode PriceCode { get; set; } = new();

        [JsonProperty("priceamount")]
        public BillingPriceAmount PriceAmount { get; set; } = new();
    }

    public class BillingProduct
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
    public class BillingPriceCode
    {
        [JsonProperty("monthly")]
        public string Monthly { get; set; } = string.Empty;

        [JsonProperty("annual")]
        public string Annual { get; set; } = string.Empty;
    }

    public class BillingPriceAmount
    {
        [JsonProperty("monthly")]
        public int Monthly { get; set; }

        [JsonProperty("annual")]
        public int Annual { get; set; }
    }
}
