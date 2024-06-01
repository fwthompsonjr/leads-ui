namespace permissions.api.tests.Entities
{
    public class SubscriptionModificationResponseTests
    {
        [Fact]
        public void ItemCanBeGenerated()
        {
            var item = PaymentHtmlHelper.ModificationFaker.Generate();
            Assert.NotNull(item);
            Assert.NotNull(item.Id);
            Assert.NotNull(item.UserId);
            Assert.NotNull(item.PaymentIntentId);
            Assert.NotNull(item.ClientSecret);
            Assert.NotNull(item.ExternalId);
            Assert.NotNull(item.Description);
            Assert.True(item.Amount.HasValue);
            Assert.NotNull(item.SuccessUrl);
        }
    }
}
