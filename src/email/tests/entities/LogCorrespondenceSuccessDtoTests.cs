using Bogus;
using legallead.email.entities;

namespace legallead.email.tests.entities
{
    public class LogCorrespondenceSuccessDtoTests
    {
        private static readonly Faker<LogCorrespondenceSuccessDto> faker =
            new Faker<LogCorrespondenceSuccessDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        [Fact]
        public void DtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DtoIsBaseDto()
        {
            var item = faker.Generate();
            Assert.IsAssignableFrom<BaseDto>(item);
        }

        [Fact]
        public void DtoHasATableName()
        {
            var item = faker.Generate();
            var tb = item.TableName;
            Assert.False(string.IsNullOrEmpty(tb));
        }

        [Theory]
        [InlineData("Id")]
        public void DtoHasExpectedFieldDefined(string name)
        {
            var sut = new LogCorrespondenceSuccessDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        public void DtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LogCorrespondenceSuccessDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }

        [Theory]
        [InlineData("UnmappedId")]
        [InlineData("")]
        public void DtoCanReadNonFields(string fieldName)
        {
            var demo = faker.Generate();
            var actual = demo[fieldName];
            Assert.Null(actual);
        }
    }
}