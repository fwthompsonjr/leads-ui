using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class StatusSummaryDtoTests
    {

        private static readonly Faker<StatusSummaryDto> faker =
            new Faker<StatusSummaryDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Region, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Count, y => y.Random.Int(0, 125000))
            .RuleFor(x => x.Oldest, y => y.Date.Recent())
            .RuleFor(x => x.Newest, y => y.Date.Recent());



        [Fact]
        public void StatusSummaryDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new StatusSummaryDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void StatusSummaryDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Region")]
        [InlineData("Count")]
        [InlineData("Oldest")]
        [InlineData("Newest")]
        public void StatusSummaryDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new StatusSummaryDto();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Region")]
        [InlineData("Count")]
        [InlineData("Oldest")]
        [InlineData("Newest")]
        public void StatusSummaryDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new StatusSummaryDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}