using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchDtoTests
    {

        private static readonly Faker<SearchDto> faker =
            new Faker<SearchDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SearchDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchDtoIsBaseDto()
        {
            var sut = new SearchDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void SearchDtoHasTableNameDefined()
        {
            var expected = "search";
            var sut = new SearchDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("Name")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("ExpectedRows")]
        [InlineData("CreateDate")]
        public void SearchDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchDto();
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
        public void SearchDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}
