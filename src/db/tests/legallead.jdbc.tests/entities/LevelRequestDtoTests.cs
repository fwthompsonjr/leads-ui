using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LevelRequestDtoTests
    {

        private static readonly Faker<LevelRequestDto> faker =
            new Faker<LevelRequestDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LevelRequestDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LevelRequestDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelRequestDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelRequestDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LevelRequestDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ExternalId")]
        [InlineData("InvoiceUri")]
        [InlineData("LevelName")]
        [InlineData("SessionId")]
        [InlineData("IsPaymentSuccess")]
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void LevelRequestDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LevelRequestDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ExternalId")]
        [InlineData("InvoiceUri")]
        [InlineData("LevelName")]
        [InlineData("SessionId")]
        [InlineData("IsPaymentSuccess")]
        [InlineData("CompletionDate")]
        [InlineData("CreateDate")]
        public void LevelRequestDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LevelRequestDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}