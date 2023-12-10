using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserProfileHistoryTests
    {
        private readonly Faker<UserProfileHistory> faker =
            new Faker<UserProfileHistory>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserProfileId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ProfileMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.GroupId, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void UserProfileHistoryCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserProfileHistory();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserProfileHistoryIsBaseDto()
        {
            var sut = new UserProfileHistory();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserProfileHistoryHasTableNameDefined()
        {
            var expected = "userProfilehistory";
            var sut = new UserProfileHistory();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void UserProfileHistoryHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserProfileId", "UserId", "ProfileMapId", "KeyValue", "KeyName", "GroupId", "CreateDate" };
            var sut = new UserProfileHistory();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserProfileId")]
        [InlineData("UserId")]
        [InlineData("ProfileMapId")]
        [InlineData("KeyValue")]
        [InlineData("KeyName")]
        [InlineData("GroupId")]
        [InlineData("CreateDate")]
        public void UserProfileHistoryHasExpectedFieldDefined(string name)
        {
            var sut = new UserProfileHistory();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateUserProfileId()
        {
            var items = faker.Generate(2);
            items[0].UserProfileId = items[1].UserProfileId;
            Assert.Equal(items[1].UserProfileId, items[0].UserProfileId);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateProfileMapId()
        {
            var items = faker.Generate(2);
            items[0].ProfileMapId = items[1].ProfileMapId;
            Assert.Equal(items[1].ProfileMapId, items[0].ProfileMapId);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateKeyValue()
        {
            var items = faker.Generate(2);
            items[0].KeyValue = items[1].KeyValue;
            Assert.Equal(items[1].KeyValue, items[0].KeyValue);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateKeyName()
        {
            var items = faker.Generate(2);
            items[0].KeyName = items[1].KeyName;
            Assert.Equal(items[1].KeyName, items[0].KeyName);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateGroupId()
        {
            var items = faker.Generate(2);
            items[0].GroupId = items[1].GroupId;
            Assert.Equal(items[1].GroupId, items[0].GroupId);
        }

        [Fact]
        public void UserProfileHistoryCanUpdateCreateDate()
        {
            var items = faker.Generate(2);
            items[0].CreateDate = items[1].CreateDate;
            Assert.Equal(items[1].CreateDate, items[0].CreateDate);
        }

        [Theory]
        [InlineData("Id", "123-456-789")]
        [InlineData("UserProfileId", "404-55-111")]
        [InlineData("UserId", "987-55-111")]
        [InlineData("ProfileMapId", "abcd-efg-hijk")]
        [InlineData("KeyValue", "party-all-the-time")]
        [InlineData("KeyName", "do-you-party")]
        [InlineData("GroupId", 1525)]
        public void UserProfileHistoryCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new UserProfileHistory();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}