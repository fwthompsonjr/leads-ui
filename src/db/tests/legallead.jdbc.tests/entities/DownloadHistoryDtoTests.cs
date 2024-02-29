using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DownloadHistoryDtoTests
    {

        private static readonly Faker<DownloadHistoryDto> faker =
            new Faker<DownloadHistoryDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.RowCount, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.InvoiceId, y => y.Hacker.Phrase())
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.AllowRollback, y => y.Random.Bool())
            .RuleFor(x => x.RollbackCount, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 1000));


        [Fact]
        public void DownloadHistoryDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DownloadHistoryDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DownloadHistoryDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("SearchId")]
        [InlineData("Price")]
        [InlineData("RowCount")]
        [InlineData("InvoiceId")]
        [InlineData("PurchaseDate")]
        [InlineData("AllowRollback")]
        [InlineData("RollbackCount")]
        [InlineData("CreateDate")]
        public void DownloadHistoryDtoHasExpectedFieldDefined(string name)
        {
            var sut = new DownloadHistoryDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("SearchId")]
        [InlineData("Price")]
        [InlineData("RowCount")]
        [InlineData("InvoiceId")]
        [InlineData("PurchaseDate")]
        [InlineData("AllowRollback")]
        [InlineData("RollbackCount")]
        [InlineData("CreateDate")]
        public void DownloadHistoryDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new DownloadHistoryDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}