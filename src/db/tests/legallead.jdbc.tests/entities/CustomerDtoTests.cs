using Bogus;
using legallead.jdbc.entities;
using Newtonsoft.Json;

namespace legallead.jdbc.tests.entities
{
    public class CustomerDtoTests
    {

        private static readonly Faker<CustomerDto> faker =
            new Faker<CustomerDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Hacker.Phrase())
            .RuleFor(x => x.Email, y => y.Person.Email);


        [Fact]
        public void CustomerDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CustomerDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void CustomerDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void CustomerDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void CustomerDtoCanSetUserName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserName = src.UserName;
            Assert.Equal(src.UserName, dest.UserName);
        }
        [Fact]
        public void CustomerDtoCanSetEmail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Email = src.Email;
            Assert.Equal(src.Email, dest.Email);
        }

        [Fact]
        public void CustomerDtoIsBaseDto()
        {
            var sut = new CustomerDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void CustomerDtoCanMapToUnMappedCustomerBo()
        {
            var sut = faker.Generate();
            var json = JsonConvert.SerializeObject(sut);
            var actual = JsonConvert.DeserializeObject<UnMappedCustomerBo>(json);
            Assert.NotNull(actual);
            Assert.Equal(sut.Id, actual.Id);
            Assert.Equal(sut.UserName, actual.UserName);
            Assert.Equal(sut.Email, actual.Email);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        public void CustomerDtoHasExpectedFieldDefined(string name)
        {
            var sut = new CustomerDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        public void CustomerDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new CustomerDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}