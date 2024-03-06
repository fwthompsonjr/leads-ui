using legallead.permissions.api.Models;
using Newtonsoft.Json;

namespace permissions.api.tests.Models
{
    public class LevelChangeResponseTests
    {
        private static readonly Faker<LevelChangeResponse> faker = new Faker<LevelChangeResponse>()
            .RuleFor(x => x.ExternalId, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool());

        [Fact]
        public void ModelCanCreate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
        [Fact]
        public void ModelCanBeSerialized()
        {
            var test = faker.Generate();
            var serialized = JsonConvert.SerializeObject(test);
            Assert.NotEmpty(serialized);
        }
    }
}