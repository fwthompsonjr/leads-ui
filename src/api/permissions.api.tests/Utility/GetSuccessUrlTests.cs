using legallead.permissions.api.Utility;

namespace permissions.api.tests.Utility
{
    public class GetSuccessUrlTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("https://localhost:44345/subscription-checkout")]
        [InlineData("https://localhost:44345/discount-checkout")]
        [InlineData("https://localhost:44345/subscription-checkout?sessionid=sub_1PLtruDhgP60CL9xqNZtXBMh&id=Foc7k135jHxo0WYC")]
        [InlineData("https://localhost:44345/discount-checkout?sessionid=sub_1PLtrJDhgP60CL9x8eWJy0U9&id=pETOsXXCUMdNIPSc")]
        public void DiscountCanGetSuccessUrl(string? invoiceUri)
        {
            var externalId = new Faker().Random.AlphaNumeric(10);
            var response = StripeDiscountRetryService.GetSuccesUrl(invoiceUri, externalId);
            Assert.NotNull(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("https://localhost:44345/subscription-checkout")]
        [InlineData("https://localhost:44345/discount-checkout")]
        [InlineData("https://localhost:44345/subscription-checkout?sessionid=sub_1PLtruDhgP60CL9xqNZtXBMh&id=Foc7k135jHxo0WYC")]
        [InlineData("https://localhost:44345/discount-checkout?sessionid=sub_1PLtrJDhgP60CL9x8eWJy0U9&id=pETOsXXCUMdNIPSc")]
        public void SubscriptionCanGetSuccessUrl(string? invoiceUri)
        {
            var externalId = new Faker().Random.AlphaNumeric(10);
            var response = StripeSubscriptionRetryService.GetSuccesUrl(invoiceUri, externalId);
            Assert.NotNull(response);
        }
    }
}
