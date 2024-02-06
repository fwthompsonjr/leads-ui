using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchQueryDtoTests
    {

        private static readonly Faker<SearchQueryDto> faker =
            new Faker<SearchQueryDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SearchQueryDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchQueryDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchQueryDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchQueryDtoIsBaseDto()
        {
            var sut = new SearchQueryDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("Name")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("ExpectedRows")]
        [InlineData("CreateDate")]
        [InlineData("SearchProgress")]
        [InlineData("StateCode")]
        [InlineData("CountyName")]
        public void SearchQueryDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchQueryDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("Name")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("ExpectedRows")]
        [InlineData("CreateDate")]
        [InlineData("SearchProgress")]
        [InlineData("StateCode")]
        [InlineData("CountyName")]
        public void SearchQueryDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchQueryDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}
