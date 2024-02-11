using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchInvoiceBoTests
    {

        private static readonly Faker<SearchInvoiceBo> faker =
            new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 25))
            .RuleFor(x => x.UnitPrice, y => y.Random.Int(1, 25))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 25))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SearchInvoiceBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchInvoiceBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchInvoiceBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

    }
}