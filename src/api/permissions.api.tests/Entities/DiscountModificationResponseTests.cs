using legallead.permissions.api.Entities;

namespace permissions.api.tests.Entities
{
    public class DiscountModificationResponseTests
    {
        [Fact]
        public void ItemCanBeCreated()
        {
            var errored = Record.Exception(() =>
            {
                var item = PaymentHtmlHelper.DiscountPaymentFaker.Generate(3);
                var modication = new DiscountModificationResponse
                {
                    Data = item
                };
                Assert.Equal(3, modication.Data.Count);
            });
            Assert.Null(errored);
        }
        [Fact]
        public void ItemIsSubscriptionModificationResponse()
        {
            var item = PaymentHtmlHelper.DiscountPaymentFaker.Generate(3);
            var modication = new DiscountModificationResponse
            {
                Data = item
            };
            Assert.IsAssignableFrom<SubscriptionModificationResponse>(modication);
        }
    }
}
