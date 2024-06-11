using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class ActiveSearchOverviewDtoTests
    {

        private static readonly Faker<ActiveSearchOverviewDto> faker =
            new Faker<ActiveSearchOverviewDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Searches, y => y.Hacker.Phrase())
            .RuleFor(x => x.Statuses, y => y.Hacker.Phrase())
            .RuleFor(x => x.Staged, y => y.Hacker.Phrase());



        [Fact]
        public void ActiveSearchOverviewDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ActiveSearchOverviewDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ActiveSearchOverviewDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Searches")]
        [InlineData("Statuses")]
        [InlineData("Staged")]
        public void ActiveSearchOverviewDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new ActiveSearchOverviewDto();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Searches")]
        [InlineData("Statuses")]
        [InlineData("Staged")]
        public void ActiveSearchOverviewDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new ActiveSearchOverviewDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}