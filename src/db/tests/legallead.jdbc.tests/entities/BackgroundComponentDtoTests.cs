using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class BackgroundComponentDtoTests
    {

        private static readonly Faker<BackgroundComponentDto> faker =
            new Faker<BackgroundComponentDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ComponentName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ServiceName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void BackgroundComponentDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new BackgroundComponentDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void BackgroundComponentDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void BackgroundComponentDtoIsBaseDto()
        {
            var sut = new BackgroundComponentDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void BackgroundComponentDtoHasTableNameDefined()
        {
            var expected = "BGCOMPONENT";
            var sut = new BackgroundComponentDto();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("ComponentName")]
        [InlineData("ServiceName")]
        [InlineData("CreateDate")]
        public void BackgroundComponentDtoHasExpectedFieldDefined(string name)
        {
            var sut = new BackgroundComponentDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("ComponentName")]
        [InlineData("ServiceName")]
        [InlineData("CreateDate")]
        public void BackgroundComponentDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new BackgroundComponentDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}