using legallead.email.entities;

namespace legallead.email.tests.entities
{
    public class UserEmailSettingDtoTests
    {
        private static readonly List<string> commonKeys =
        [
            "Email 1",
            "Email 2",
            "Email 3",
            "First Name",
            "Last Name",
        ];
        private static readonly Faker<UserEmailSettingDto> faker =
            new Faker<UserEmailSettingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.KeyName, y => y.PickRandom(commonKeys))
            .FinishWith((a, b) =>
            {
                b.KeyValue = b.KeyName switch
                {
                    "First Name" => a.Person.FirstName,
                    "Last Name" => a.Person.LastName,
                    _ => a.Person.Email
                };
            });

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
        [InlineData("KeyName")]
        [InlineData("KeyValue")]
        public void DtoHasExpectedFieldDefined(string name)
        {
            var sut = new UserEmailSettingDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Email")]
        [InlineData("UserName")]
        [InlineData("KeyName")]
        [InlineData("KeyValue")]
        public void DtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new UserEmailSettingDto();
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
