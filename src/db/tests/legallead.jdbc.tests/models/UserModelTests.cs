using Bogus;
using legallead.jdbc.models;

namespace legallead.jdbc.tests.models
{
    public class UserModelTests
    {
        private static readonly Faker<UserModel> faker =
            new Faker<UserModel>()
            .RuleFor(x => x.UserName, y => y.Company.CompanyName())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(28));

        [Fact]
        public void UserModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserModel();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserModelCanUpdateUserName()
        {
            var items = faker.Generate(2);
            items[0].UserName = items[1].UserName;
            Assert.Equal(items[1].UserName, items[0].UserName);
        }

        [Fact]
        public void UserModelCanUpdateEmail()
        {
            var items = faker.Generate(2);
            items[0].Email = items[1].Email;
            Assert.Equal(items[1].Email, items[0].Email);
        }

        [Fact]
        public void UserModelCanUpdatePassword()
        {
            var items = faker.Generate(2);
            items[0].Password = items[1].Password;
            Assert.Equal(items[1].Password, items[0].Password);
        }

        [Fact]
        public void UserModelCanConvertToUser()
        {
            var item = faker.Generate();
            var conversion = UserModel.ToUser(item);
            Assert.Equal(item.UserName, conversion.UserName);
            Assert.Equal(item.Email, conversion.Email);
            Assert.False(string.IsNullOrEmpty(conversion.PasswordHash));
            Assert.False(string.IsNullOrEmpty(conversion.PasswordSalt));
        }
    }
}