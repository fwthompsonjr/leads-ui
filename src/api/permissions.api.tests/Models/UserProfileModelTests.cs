using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class UserProfileModelTests
    {
        private static readonly Faker<UserProfileModel> faker =
            new Faker<UserProfileModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.KeyName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.KeyValue, y => y.Random.AlphaNumeric(30));

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserProfileModel();
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
        public void ModelCanGetKeyName()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.KeyName));
        }

        [Fact]
        public void ModelCanSetKeyName()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.KeyName = source.KeyName;
            Assert.Equal(source.KeyName, test.KeyName);
        }

        [Fact]
        public void ModelCanGetKeyValue()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.KeyValue));
        }

        [Fact]
        public void ModelCanSetKeyValue()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.KeyValue = source.KeyValue;
            Assert.Equal(source.KeyValue, test.KeyValue);
        }
    }
}