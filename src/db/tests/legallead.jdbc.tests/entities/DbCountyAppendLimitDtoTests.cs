using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyAppendLimitDtoTests
    {

        private static readonly Faker<DbCountyAppendLimitDto> faker =
            new Faker<DbCountyAppendLimitDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DbCountyAppendLimitDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void ModelIsBaseDto()
        {
            var sut = new DbCountyAppendLimitDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new DbCountyAppendLimitDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}