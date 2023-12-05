using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserTests
    {
        private static readonly Faker<User> faker =
            new Faker<User>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Company.CompanyName())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.PasswordHash, y => y.Random.AlphaNumeric(60))
            .RuleFor(x => x.PasswordSalt, y => y.Random.AlphaNumeric(30));

        [Fact]
        public void UserCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new User();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserProfileIsBaseDto()
        {
            var sut = new User();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserHasTableNameDefined()
        {
            var expected = "users";
            var sut = new User();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void UserHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserName", "Email", "PasswordHash", "PasswordSalt" };
            var sut = new User();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("Email")]
        [InlineData("PasswordHash")]
        [InlineData("PasswordSalt")]
        public void UserHasExpectedFieldDefined(string name)
        {
            var sut = new User();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserCanUpdateUserName()
        {
            var items = faker.Generate(2);
            items[0].UserName = items[1].UserName;
            Assert.Equal(items[1].UserName, items[0].UserName);
        }

        [Fact]
        public void UserCanUpdateEmail()
        {
            var items = faker.Generate(2);
            items[0].Email = items[1].Email;
            Assert.Equal(items[1].Email, items[0].Email);
        }

        [Fact]
        public void UserCanUpdatePasswordHash()
        {
            var items = faker.Generate(2);
            items[0].PasswordHash = items[1].PasswordHash;
            Assert.Equal(items[1].PasswordHash, items[0].PasswordHash);
        }

        [Fact]
        public void UserCanUpdatePasswordSalt()
        {
            var items = faker.Generate(2);
            items[0].PasswordSalt = items[1].PasswordSalt;
            Assert.Equal(items[1].PasswordSalt, items[0].PasswordSalt);
        }

        [Theory]
        [InlineData(0, "abcdefg")]
        [InlineData(1, "abcdefg")]
        [InlineData(2, "abcdefg")]
        [InlineData(3, "abcdefg")]
        [InlineData(4, "abcdefg")]
        public void UserCanReadWriteByIndex(int position, object expected)
        {
            var sut = new User();
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}