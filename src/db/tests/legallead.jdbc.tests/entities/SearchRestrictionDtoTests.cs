using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchRestrictionDtoTests
    {

        private static readonly Faker<SearchRestrictionDto> faker =
            new Faker<SearchRestrictionDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.Reason, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MaxPerMonth, y => y.Random.Int(1, 100000))
            .RuleFor(x => x.MaxPerYear, y => y.Random.Int(1, 100000))
            .RuleFor(x => x.ThisMonth, y => y.Random.Int(1, 100000))
            .RuleFor(x => x.ThisYear, y => y.Random.Int(1, 100000));

        [Fact]
        public void SearchRestrictionDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchRestrictionDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchRestrictionDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchRestrictionDtoIsBaseDto()
        {
            var sut = new SearchRestrictionDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("IsLocked")]
        [InlineData("Reason")]
        [InlineData("MaxPerMonth")]
        [InlineData("MaxPerYear")]
        [InlineData("ThisMonth")]
        [InlineData("ThisYear")]
        public void SearchRestrictionDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchRestrictionDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("IsLocked")]
        [InlineData("Reason")]
        [InlineData("MaxPerMonth")]
        [InlineData("MaxPerYear")]
        [InlineData("ThisMonth")]
        [InlineData("ThisYear")]
        public void SearchRestrictionDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchRestrictionDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}