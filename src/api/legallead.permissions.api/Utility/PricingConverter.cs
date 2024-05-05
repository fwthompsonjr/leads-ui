using Stripe.Checkout;

namespace legallead.permissions.api.Utility
{
    public static class PricingConverter
    {
        public static SessionLineItemOptions ConvertFrom(SearchInvoiceBo source, User user)
        {
            var priceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = Convert.ToInt64(source.UnitPrice * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = GetPricingName(source),
                    Description = source.ItemType,
                    Metadata = new()
                    {
                        { "external-id", source.ExternalId },
                        { "user-name", user.UserName },
                        { "user-email", user.Email },
                        { "payment-type", "Record Search" },
                    },
                },
                TaxBehavior = "inclusive"
            };
            return new()
            {
                PriceData = priceData,
                Quantity = source.ItemCount.GetValueOrDefault(0)
            };
        }

        private static string GetPricingName(SearchInvoiceBo source)
        {
            const string name = "Ad-Hoc";
            var hasLineNumber = int.TryParse(source.LineId, out var line);
            if (!hasLineNumber) return name;
            return line switch
            {
                0 => "Record Search",
                1 => "County Discount",
                2 => "State Discount",
                _ => name,
            };
        }
    }
}
