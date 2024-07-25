using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class WorkingSearchDtoTests
    {

        private static readonly Faker<WorkingSearchDto> faker =
            new Faker<WorkingSearchDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MessageId, y => y.Random.Int(0, 6))
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 2))
            .RuleFor(x => x.MachineName, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent());



        [Fact]
        public void WorkingSearchDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new WorkingSearchDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkingSearchDtoCanBeGenerated()
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
        [InlineData("MessageId")]
        [InlineData("StatusId")]
        [InlineData("MachineName")]
        [InlineData("CreateDate")]
        [InlineData("LastUpdateDt")]
        [InlineData("CompletionDate")]
        public void WorkingSearchDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new WorkingSearchDto();
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
        [InlineData("MessageId")]
        [InlineData("StatusId")]
        [InlineData("MachineName")]
        [InlineData("CreateDate")]
        [InlineData("LastUpdateDt")]
        [InlineData("CompletionDate")]
        public void WorkingSearchDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new WorkingSearchDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}