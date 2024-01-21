using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchQueueDtoTests
    {

        private static readonly Faker<SearchQueueDto> faker =
            new Faker<SearchQueueDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.Payload, y => y.Random.Int(5, 25055).ToString())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SearchQueueDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchQueueDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchQueueDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void SearchQueueDtoIsBaseDto()
        {
            var sut = new SearchQueueDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void SearchQueueDtoHasTableNameDefined()
        {
            var expected = "SEARCHQUEUE";
            var sut = new SearchQueueDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("Name")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("ExpectedRows")]
        [InlineData("Payload")]
        [InlineData("CreateDate")]
        public void SearchQueueDtoHasExpectedFieldDefined(string name)
        {
            var sut = new SearchQueueDto();
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
        [InlineData("Payload")]
        public void SearchQueueDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new SearchQueueDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}