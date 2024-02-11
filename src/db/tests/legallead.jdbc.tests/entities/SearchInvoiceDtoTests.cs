using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchInvoiceDtoTests
    {

        private static readonly Faker<SearchInvoiceDto> faker =
            new Faker<SearchInvoiceDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
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
        public void SearchInvoiceDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchInvoiceDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchInvoiceDtoCanBeGenerated()
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
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        public void SearchInvoiceDtoCanWriteAndRead(int fieldId)
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
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void SearchInvoiceDtoCanWriteAndReadByFieldName(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                var name = a.FieldList[fieldId];
                a[name] = b[name];
                Assert.Equal(a[name], b[name]);
            });
            Assert.Null(exception);
        }
    }
}