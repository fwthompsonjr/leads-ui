using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class SearchRequestDtoTests
    {

        private static readonly Faker<SearchRequestDto> faker =
            new Faker<SearchRequestDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.Line, y =>
            {
                var phrase = y.Hacker.Phrase();
                return Encoding.UTF8.GetBytes(phrase);
            })
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void SearchRequestDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchRequestDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchRequestDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchRequestDtoIsBaseDto()
        {
            var sut = new SearchRequestDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void SearchResponseDtoIsSearchRequestDtoIsBaseDto()
        {
            var sut = new SearchResponseDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
            Assert.IsAssignableFrom<SearchRequestDto>(sut);
        }

        [Fact]
        public void SearchDetailDtoIsSearchRequestDtoIsBaseDto()
        {
            var sut = new SearchDetailDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
            Assert.IsAssignableFrom<SearchRequestDto>(sut);
        }

        [Fact]
        public void SearchRequestDtoHasTableNameDefined()
        {
            var expected = "searchrequest";
            var sut = new SearchRequestDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void SearchDetailDtoHasTableNameDefined()
        {
            var expected = "searchdetail";
            var sut = new SearchDetailDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void SearchResponseDtoHasTableNameDefined()
        {
            var expected = "searchresponse";
            var sut = new SearchResponseDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("SearchId")]
        [InlineData("LineNbr")]
        [InlineData("Line")]
        [InlineData("CreateDate")]
        public void SearchRequestDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchRequestDto();
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
        public void SearchRequestDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchRequestDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}
