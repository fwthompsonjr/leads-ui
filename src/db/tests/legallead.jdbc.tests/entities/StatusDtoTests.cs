using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class StatusDtoTests
    {

        private static readonly Faker<StatusDto> faker =
            new Faker<StatusDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Total, y => y.Random.Int(0, 125000));



        [Fact]
        public void StatusDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new StatusDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void StatusDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchProgress")]
        [InlineData("Total")]
        public void StatusDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new StatusDto();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchProgress")]
        [InlineData("Total")]
        public void StatusDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new StatusDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}