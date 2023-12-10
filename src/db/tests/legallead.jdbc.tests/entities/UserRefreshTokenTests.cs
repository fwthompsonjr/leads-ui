using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserRefreshTokenTests
    {
        private readonly Faker<UserRefreshToken> faker =
            new Faker<UserRefreshToken>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.RefreshToken, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void UserRefreshTokenCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserRefreshToken();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserRefreshTokenIsBaseDto()
        {
            var sut = new UserRefreshToken();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserRefreshTokenHasTableNameDefined()
        {
            var expected = "usertokens";
            var sut = new UserRefreshToken();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void UserRefreshTokenHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserId", "RefreshToken", "IsActive", "CreateDate" };
            var sut = new UserRefreshToken();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("RefreshToken")]
        [InlineData("IsActive")]
        [InlineData("CreateDate")]
        public void UserRefreshTokenHasExpectedFieldDefined(string name)
        {
            var sut = new UserRefreshToken();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserRefreshTokenCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserRefreshTokenCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserRefreshTokenCanUpdateRefreshToken()
        {
            var items = faker.Generate(2);
            items[0].RefreshToken = items[1].RefreshToken;
            Assert.Equal(items[1].RefreshToken, items[0].RefreshToken);
        }

        [Fact]
        public void UserRefreshTokenCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }

        [Theory]
        [InlineData("Id", "123-456-789")]
        [InlineData("UserId", "987-55-111")]
        [InlineData("RefreshToken", "abcd-efg-hijk")]
        [InlineData("IsActive", false)]
        public void UserRefreshTokenCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new UserRefreshToken();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UserRefreshTokenCanReadWriteCreateDate()
        {
            string fieldName = "CreateDate";
            var expected = faker.Generate().CreateDate;
            var sut = new UserRefreshToken();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}