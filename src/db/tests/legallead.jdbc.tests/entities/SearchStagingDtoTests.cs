using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class SearchStagingDtoTests
    {

        private static readonly Faker<SearchStagingDto> faker =
            new Faker<SearchStagingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StagingType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(1, 100000))
            .RuleFor(x => x.LineData, y => {
                var m = y.Hacker.Phrase();
                return Encoding.UTF8.GetBytes(m);
            })
            .RuleFor(x => x.LineText, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsBinary, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SearchStagingDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchStagingDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchStagingDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchStagingDtoIsBaseDto()
        {
            var sut = new SearchStagingDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void SearchStagingDtoHasTableNameDefined()
        {
            var expected = "SEARCHSTAGING";
            var sut = new SearchStagingDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("StagingType")]
        [InlineData("LineNbr")]
        [InlineData("LineData")]
        [InlineData("LineText")]
        [InlineData("IsBinary")]
        [InlineData("CreateDate")]
        public void SearchStagingDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchStagingDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("StagingType")]
        [InlineData("LineNbr")]
        [InlineData("LineData")]
        [InlineData("LineText")]
        [InlineData("IsBinary")]
        [InlineData("CreateDate")]
        public void SearchStagingDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchStagingDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}