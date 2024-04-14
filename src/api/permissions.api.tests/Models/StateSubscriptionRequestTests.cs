using legallead.models;

namespace permissions.api.tests.Models
{
    public class StateSubscriptionRequestTests
    {
        private static readonly Faker<StateSubscriptionRequest> faker = new Faker<StateSubscriptionRequest>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(15));

        [Fact]
        public void ModelCanBeCreated()
        {
            var model = faker.Generate();
            Assert.False(string.IsNullOrEmpty(model.Name));
        }
    }
}
