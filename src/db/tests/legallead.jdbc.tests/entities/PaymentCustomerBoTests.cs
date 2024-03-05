using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PaymentCustomerBoTests
    {

        private static readonly Faker<PaymentCustomerBo> faker =
            new Faker<PaymentCustomerBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Hacker.Phrase())
            .RuleFor(x => x.Email, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsTest, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void PaymentCustomerBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PaymentCustomerBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void PaymentCustomerBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void PaymentCustomerBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }
        [Fact]
        public void PaymentCustomerBoCanSetCustomerId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CustomerId = src.CustomerId;
            Assert.Equal(src.CustomerId, dest.CustomerId);
        }

        [Fact]
        public void PaymentCustomerBoCanSetEmail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Email = src.Email;
            Assert.Equal(src.Email, dest.Email);
        }

        [Fact]
        public void PaymentCustomerBoCanSetIsTest()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsTest = src.IsTest;
            Assert.Equal(src.IsTest, dest.IsTest);
        }
    }
}