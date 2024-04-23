using Bogus;
using legallead.email.models;

namespace legallead.email.tests.models
{
    public class DbSettingTests
    {
        private static readonly Faker<DbSetting> faker =
            new Faker<DbSetting>()
            .RuleFor(x => x.Server, y => y.Internet.DomainName())
            .RuleFor(x => x.DataBase, y => y.Internet.DomainWord())
            .RuleFor(x => x.Code, y => y.Random.AlphaNumeric(45))
            .RuleFor(x => x.Key, y => y.Random.AlphaNumeric(15));
        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
    }
}
