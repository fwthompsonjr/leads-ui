using legallead.permissions.api.Model;
using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class PaymentDisplayClientTests
    {
        private static readonly Faker<PaymentDisplayClient> faker =
            new Faker<PaymentDisplayClient>()
            .RuleFor(x => x.SessionId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ClientId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IntentId, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PaymentDisplayClient();
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var control = new PaymentDisplayClient();
            if (fieldId == 0) Assert.NotEqual(control.SessionId, test.SessionId);
            if (fieldId == 1) Assert.NotEqual(control.ClientId, test.ClientId);
            if (fieldId == 2) Assert.NotEqual(control.IntentId, test.IntentId);
        }
    }
}