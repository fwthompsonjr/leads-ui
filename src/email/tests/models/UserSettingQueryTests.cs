using Bogus;
using legallead.email.models;

namespace legallead.email.tests.entities
{
    public class UserSettingQueryTests
    {
        private static readonly Faker<UserSettingQuery> faker =
            new Faker<UserSettingQuery>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email);

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
            var sut = new UserSettingQuery();
            var test = faker.Generate();
            Assert.NotEqual(sut.Id, test.Id);
            Assert.NotEqual(sut.Email, test.Email);
        }
    }
}