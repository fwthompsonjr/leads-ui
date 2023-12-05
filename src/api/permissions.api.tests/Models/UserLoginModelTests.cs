using legallead.permissions.api;
using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class UserLoginModelTests
    {
        private static readonly Faker<UserLoginModel> faker =
            new Faker<UserLoginModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(22));

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserLoginModel();
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
        public void ModelCanGetUserName()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.UserName));
        }

        [Fact]
        public void ModelCanSetUserName()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.UserName = source.UserName;
            Assert.Equal(source.UserName, test.UserName);
        }

        [Fact]
        public void ModelCanGetPassword()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.Password));
        }

        [Fact]
        public void ModelCanSetPassword()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Password = source.Password;
            Assert.Equal(source.Password, test.Password);
        }

        [Fact]
        public void ModelIsInValidWithEmptyUserName()
        {
            var test = faker.Generate();
            test.UserName = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ModelIsInValidWithEmptyPassword()
        {
            var test = faker.Generate();
            test.Password = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }
    }
}