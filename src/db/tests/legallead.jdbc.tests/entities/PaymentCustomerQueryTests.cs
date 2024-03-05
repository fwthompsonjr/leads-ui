using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PaymentCustomerQueryTests
    {

        private static readonly Faker<PaymentCustomerQuery> faker =
            new Faker<PaymentCustomerQuery>()
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.AccountType, y => y.Hacker.Phrase());


        [Fact]
        public void PaymentCustomerQueryCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PaymentCustomerQuery();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerQueryCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerQueryCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }
        [Fact]
        public void PaymentCustomerQueryCanSetAccountType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.AccountType = src.AccountType;
            Assert.Equal(src.AccountType, dest.AccountType);
        }

        [Fact]
        public void PaymentCustomerQueryCanGetParameters()
        {
            var data = faker.Generate();
            var parms = data.GetParameters();
            Assert.NotNull(parms);
            Assert.Contains("user_index", parms.ParameterNames);
            Assert.Contains("account_type", parms.ParameterNames);
            Assert.Equal(2, parms.ParameterNames.Count());
        }
    }
}