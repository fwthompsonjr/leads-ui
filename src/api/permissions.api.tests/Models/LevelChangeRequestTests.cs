using legallead.permissions.api.Models;
using Newtonsoft.Json;

namespace permissions.api.tests.Models
{
    public class LevelChangeRequestTests
    {
        private static readonly List<string> LevelNames = new()
        {
            "admin",
            "guest",
            "platinum",
            "silver",
            "gold"
        };

        private static Faker<LevelChangeRequest> faker = new Faker<LevelChangeRequest>()
            .RuleFor(x => x.ExternalId, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.InvoiceUri, y => y.Internet.Url())
            .RuleFor(x => x.LevelName, y => y.PickRandom(LevelNames))
            .RuleFor(x => x.SessionId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString());

        [Fact]
        public void RequestCanCreate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
        [Fact]
        public void RequestCanBeSerialized()
        {
            var test = faker.Generate();
            var serialized = JsonConvert.SerializeObject(test);
            Assert.NotEmpty(serialized);
        }
    }
}
