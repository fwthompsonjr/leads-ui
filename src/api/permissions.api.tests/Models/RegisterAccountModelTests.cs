using legallead.permissions.api;
using legallead.permissions.api.Model;
using System.Text;

namespace permissions.api.tests.Models
{
    public class RegisterAccountModelTests
    {
        private static readonly Faker<RegisterAccountModel> faker =
            new Faker<RegisterAccountModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => AppendSpecialCharacter(y.Random.AlphaNumeric(22)))
            .RuleFor(x => x.Email, y => y.Person.Email);

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new RegisterAccountModel();
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
        public void ModelCanGetEmail()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.Email));
        }

        [Fact]
        public void ModelCanSetEmail()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Email = source.Email;
            Assert.Equal(source.Email, test.Email);
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

        [Fact]
        public void ModelIsInValidWithEmptyEmail()
        {
            var test = faker.Generate();
            test.Email = string.Empty;
            var results = test.Validate(out bool isValid);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void ModelDefaultIsValid()
        {
            var test = faker.Generate();
            var results = test.Validate(out bool isValid);
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2589:Boolean expressions should not be gratuitous",
            Justification = "Lint eror. Int variable is incremented")]
        private static string AppendSpecialCharacter(string source)
        {
            const string special = ".!#$%^*_+-=";
            var tmpFaker = new Faker();
            var charlist = special.ToCharArray();
            if (string.IsNullOrWhiteSpace(source)) return source;
            var items = source.ToCharArray();
            var builder = new StringBuilder();
            var additions = 0;
            var upperCase = 0;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (!char.IsDigit(item) && upperCase < 2)
                {
                    builder.Append(tmpFaker.Lorem.Word()[..1].ToUpper());
                    upperCase++;
                }
                builder.Append(items[i]);
                if (additions > 2) continue;
                builder.Append(tmpFaker.PickRandom(charlist));
                additions++;
            }
            return builder.ToString();
        }
    }
}