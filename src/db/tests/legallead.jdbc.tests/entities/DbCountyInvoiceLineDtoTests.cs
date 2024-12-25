using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyInvoiceLineDtoTests
    {
        [Fact]
        public void ModelCanCreate()
        {
            var error = Record.Exception(() =>
            {
                _ = dfaker.Generate();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("InvoiceId")]
        [InlineData("LineNbr")]
        [InlineData("Description")]
        [InlineData("ItemCount")]
        [InlineData("Price")]
        [InlineData("Total")]
        public void ModelHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = dfaker.Generate();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }


        [Theory]
        [InlineData("Id")]
        [InlineData("InvoiceId")]
        [InlineData("LineNbr")]
        [InlineData("Description")]
        [InlineData("ItemCount")]
        [InlineData("Price")]
        [InlineData("Total")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }

        private static readonly Faker<DbCountyInvoiceLineDto> dfaker
            = new Faker<DbCountyInvoiceLineDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.InvoiceId, y => y.Random.Int(1, 555555).ToString())
            .RuleFor(x => x.LineNbr, y => y.Random.Int(1, 100))
            .RuleFor(x => x.Description, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 100))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.Total, y => y.Random.Int(1, 555555));
    }
}