using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserProfileTests
    {
        private readonly Faker<UserProfile> faker =
            new Faker<UserProfile>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ProfileMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Company.CompanyName());

        [Fact]
        public void UserProfileCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserProfile();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserProfileIsBaseDto()
        {
            var sut = new UserProfile();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserProfileHasTableNameDefined()
        {
            var expected = "userprofile";
            var sut = new UserProfile();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void UserProfileHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserId", "ProfileMapId", "KeyValue" };
            var sut = new UserProfile();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("ProfileMapId")]
        [InlineData("KeyValue")]
        public void UserProfileHasExpectedFieldDefined(string name)
        {
            var sut = new UserProfile();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserProfileCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserProfileCanUpdateProfileMapId()
        {
            var items = faker.Generate(2);
            items[0].ProfileMapId = items[1].ProfileMapId;
            Assert.Equal(items[1].ProfileMapId, items[0].ProfileMapId);
        }

        [Fact]
        public void UserProfileCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserProfileCanUpdateKeyValue()
        {
            var items = faker.Generate(2);
            items[0].KeyValue = items[1].KeyValue;
            Assert.Equal(items[1].KeyValue, items[0].KeyValue);
        }

        [Theory]
        [InlineData(0, "abcdefg")]
        [InlineData(1, "abcdefg")]
        [InlineData(2, "abcdefg")]
        [InlineData(3, "abcdefg")]
        public void UserProfileCanReadWriteByIndex(int position, object expected)
        {
            var sut = new UserProfile();
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}