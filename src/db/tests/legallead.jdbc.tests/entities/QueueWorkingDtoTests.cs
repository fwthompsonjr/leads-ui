using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class QueueWorkingDtoTests
    {

        private static readonly Faker<QueueWorkingDto> faker =
            new Faker<QueueWorkingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2000))
            .RuleFor(x => x.MachineName, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300))
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent(15))
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent(15));



        [Fact]
        public void QueueWorkingDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new QueueWorkingDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueWorkingDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("Message")]
        [InlineData("StatusId")]
        [InlineData("MachineName")]
        [InlineData("CreateDate")]
        [InlineData("LastUpdateDt")]
        [InlineData("CompletionDate")]
        public void QueueWorkingDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new QueueWorkingDto();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("Message")]
        [InlineData("StatusId")]
        [InlineData("MachineName")]
        [InlineData("CreateDate")]
        [InlineData("LastUpdateDt")]
        [InlineData("CompletionDate")]
        public void QueueWorkingDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new QueueWorkingDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}