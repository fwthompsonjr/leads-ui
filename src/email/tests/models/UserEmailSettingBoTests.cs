using legallead.email.models;

namespace legallead.email.tests.entities
{
    public class UserEmailSettingBoTests
    {
        private static readonly List<string> commonKeys =
        [
            "Email 1",
            "Email 2",
            "Email 3",
            "First Name",
            "Last Name",
        ];
        private static readonly Faker<UserEmailSettingBo> faker =
            new Faker<UserEmailSettingBo>()
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
        public void DtoHasExpectedFieldDefined()
        {
            var sut = new UserEmailSettingBo();
            var test = faker.Generate();
            Assert.NotEqual(sut.Id, test.Id);
            Assert.NotEqual(sut.Email, test.Email);
            Assert.NotEqual(sut.UserName, test.UserName);
            Assert.NotEqual(sut.KeyName, test.KeyName);
            Assert.NotEqual(sut.KeyValue, test.KeyValue);
        }
    }
}