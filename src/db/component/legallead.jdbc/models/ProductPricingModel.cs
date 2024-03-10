using Newtonsoft.Json;

namespace legallead.jdbc.models
{

    public class ProductPricingModel
    {
        [JsonProperty("product")]
        public BillingProductModel Product { get; set; } = new();

        [JsonProperty("pricecode")]
        public BillingPriceCodeModel PriceCode { get; set; } = new();

        [JsonProperty("priceamount")]
        public BillingPriceAmountModel PriceAmount { get; set; } = new();
    }

    public class BillingProductModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
    public class BillingPriceCodeModel
    {
        [JsonProperty("monthly")]
        public string Monthly { get; set; } = string.Empty;

        [JsonProperty("annual")]
        public string Annual { get; set; } = string.Empty;
    }

    public class BillingPriceAmountModel
    {
        [JsonProperty("monthly")]
        public int Monthly { get; set; }

        [JsonProperty("annual")]
        public int Annual { get; set; }
    }

}
