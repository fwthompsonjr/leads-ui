using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PurchaseSummaryDtoTests
    {

        private static readonly Faker<PurchaseSummaryDto> faker =
            new Faker<PurchaseSummaryDto>()
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 25))
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Random.Guid().ToString("D"));

        [Fact]
        public void PurchaseSummaryDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PurchaseSummaryDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PurchaseSummaryDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void PurchaseSummaryDtoCanWriteAndRead(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                a[fieldId] = b[fieldId];
                Assert.Equal(a[fieldId], b[fieldId]);
            });
            Assert.Null(exception);
        }

    }
}