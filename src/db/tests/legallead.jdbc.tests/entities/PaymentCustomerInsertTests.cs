using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PaymentCustomerInsertTests
    {

        private static readonly Faker<PaymentCustomerInsert> faker =
            new Faker<PaymentCustomerInsert>()
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.AccountType, y => y.Hacker.Phrase());


        [Fact]
        public void PaymentCustomerInsertCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PaymentCustomerInsert();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerInsertCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerInsertCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void PaymentCustomerInsertCanSetCustomerId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CustomerId = src.CustomerId;
            Assert.Equal(src.CustomerId, dest.CustomerId);
        }
        [Fact]
        public void PaymentCustomerInsertCanSetAccountType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.AccountType = src.AccountType;
            Assert.Equal(src.AccountType, dest.AccountType);
        }

        [Fact]
        public void PaymentCustomerInsertCanGetParameters()
        {
            var data = faker.Generate();
            var parms = data.GetParameters();
            Assert.NotNull(parms);
            Assert.Contains("user_index", parms.ParameterNames);
            Assert.Contains("customer_index", parms.ParameterNames);
            Assert.Contains("account_type", parms.ParameterNames);
            Assert.Equal(3, parms.ParameterNames.Count());
        }
    }
}