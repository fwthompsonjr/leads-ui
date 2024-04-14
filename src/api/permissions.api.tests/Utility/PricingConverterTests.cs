using legallead.jdbc.entities;
using legallead.permissions.api.Utility;

namespace permissions.api.tests.Utility
{
    public class PricingConverterTests
    {

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("Other")]
        public void ConvertCanTranslate(string lineId)
        {
            var model = invoiceFaker.Generate();
            model.LineId = lineId;
            var translation = PricingConverter.ConvertFrom(model, new() { UserName = "abc", Email = "testing@test.com" });
            Assert.NotNull(translation);
        }

        private static readonly Faker<SearchInvoiceBo> invoiceFaker
            = new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 5))
            .RuleFor(x => x.UnitPrice, y => y.Random.Decimal(1, 10))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .FinishWith((a, b) =>
            {
                b.Price = b.UnitPrice.GetValueOrDefault() * b.ItemCount.GetValueOrDefault();
            });
    }
}
