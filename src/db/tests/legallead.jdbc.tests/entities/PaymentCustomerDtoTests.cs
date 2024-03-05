using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class PaymentCustomerDtoTests
    {

        private static readonly Faker<PaymentCustomerDto> faker =
            new Faker<PaymentCustomerDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Hacker.Phrase())
            .RuleFor(x => x.Email, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsTest, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void PaymentCustomerDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PaymentCustomerDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PaymentCustomerDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void PaymentCustomerDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void PaymentCustomerDtoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }
        [Fact]
        public void PaymentCustomerDtoCanSetCustomerId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CustomerId = src.CustomerId;
            Assert.Equal(src.CustomerId, dest.CustomerId);
        }

        [Fact]
        public void PaymentCustomerDtoCanSetEmail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Email = src.Email;
            Assert.Equal(src.Email, dest.Email);
        }

        [Fact]
        public void PaymentCustomerDtoCanSetIsTest()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsTest = src.IsTest;
            Assert.Equal(src.IsTest, dest.IsTest);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("CustomerId")]
        [InlineData("Email")]
        [InlineData("IsTest")]
        [InlineData("CreateDate")]
        public void PaymentCustomerDtoHasExpectedFieldDefined(string name)
        {
            var sut = new PaymentCustomerDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("CustomerId")]
        [InlineData("Email")]
        [InlineData("IsTest")]
        [InlineData("CreateDate")]
        public void PaymentCustomerDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new PaymentCustomerDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}
