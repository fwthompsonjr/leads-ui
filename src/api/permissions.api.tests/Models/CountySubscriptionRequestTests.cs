using legallead.models;

namespace permissions.api.tests.Models
{
    public class CountySubscriptionRequestTests
    {
        private static readonly Faker<CountySubscriptionRequest> faker =
            new Faker<CountySubscriptionRequest>()
            .RuleFor(x => x.County, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.State, y => y.Random.AlphaNumeric(22));

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CountySubscriptionRequest();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetCounty()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.County));
        }

        [Fact]
        public void ModelCanSetCounty()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.County = source.County;
            Assert.Equal(source.County, test.County);
        }

        [Fact]
        public void ModelCanGetState()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.State));
        }

        [Fact]
        public void ModelCanSetState()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.State = source.State;
            Assert.Equal(source.State, test.State);
        }
    }
}