using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class InvoiceDescriptionDtoTests
    {

        private static readonly Faker<InvoiceDescriptionDto> faker =
            new Faker<InvoiceDescriptionDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ItemDescription, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.County, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateAbbr, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartingDt, y => y.Date.Recent())
            .RuleFor(x => x.EndingDt, y => y.Date.Recent())
            .RuleFor(x => x.RequestDt, y => y.Date.Recent());

        [Fact]
        public void InvoiceDescriptionDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new InvoiceDescriptionDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void InvoiceDescriptionDtoCanBeGenerated()
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
        public void InvoiceDescriptionDtoCanWriteAndRead(int fieldId)
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