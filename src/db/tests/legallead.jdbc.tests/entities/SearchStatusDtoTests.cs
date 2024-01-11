using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchStatusDtoTests
    {

        private static readonly Faker<SearchStatusDto> faker =
            new Faker<SearchStatusDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.Line, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void SearchStatusDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchStatusDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchStatusDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchStatusDtoIsBaseDto()
        {
            var sut = new SearchStatusDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void SearchStatusDtoHasTableNameDefined()
        {
            var expected = "searchstatus";
            var sut = new SearchStatusDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("LineNbr")]
        [InlineData("Line")]
        [InlineData("CreateDate")]
        public void SearchStatusDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchStatusDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("LineNbr")]
        [InlineData("Line")]
        [InlineData("CreateDate")]
        public void SearchStatusDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchStatusDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}