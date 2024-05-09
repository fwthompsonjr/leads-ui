using legallead.email.entities;

namespace legallead.email.tests.entities
{
    public class GetUserAccountByEmailDtoTests
    {
        private static readonly Faker<GetUserAccountByEmailDto> faker =
            new Faker<GetUserAccountByEmailDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName);

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
        [InlineData("Email")]
        [InlineData("UserName")]
        public void DtoHasExpectedFieldDefined(string name)
        {
            var sut = new GetUserAccountByEmailDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Email")]
        [InlineData("UserName")]
        public void DtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new GetUserAccountByEmailDto();
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