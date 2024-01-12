using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchTargetDtoTests
    {

        private static readonly Faker<SearchTargetDto> faker =
            new Faker<SearchTargetDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Component, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.Line, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void SearchTargetDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchTargetDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchTargetDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchTargetDtoIsBaseDto()
        {
            var sut = new SearchTargetDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void SearchTargetDtoHasTableNameDefined()
        {
            var expected = "searchtargetdto";
            var sut = new SearchTargetDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Component")]
        [InlineData("SearchId")]
        [InlineData("LineNbr")]
        [InlineData("Line")]
        [InlineData("CreateDate")]
        public void SearchTargetDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchTargetDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Component")]
        [InlineData("SearchId")]
        [InlineData("LineNbr")]
        [InlineData("Line")]
        [InlineData("CreateDate")]
        public void SearchTargetDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchTargetDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}