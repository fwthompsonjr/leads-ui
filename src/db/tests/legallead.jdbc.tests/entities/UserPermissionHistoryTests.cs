using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserPermissionHistoryTests
    {
        private readonly Faker<UserPermissionHistory> faker =
            new Faker<UserPermissionHistory>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserPermissionId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.GroupId, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void UserPermissionHistoryCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserPermissionHistory();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserPermissionHistoryIsBaseDto()
        {
            var sut = new UserPermissionHistory();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserPermissionHistoryHasTableNameDefined()
        {
            var expected = "userpermissionhistory";
            var sut = new UserPermissionHistory();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void UserPermissionHistoryHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserPermissionId", "UserId", "PermissionMapId", "KeyValue", "KeyName", "GroupId", "CreateDate" };
            var sut = new UserPermissionHistory();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserPermissionId")]
        [InlineData("UserId")]
        [InlineData("PermissionMapId")]
        [InlineData("KeyValue")]
        [InlineData("KeyName")]
        [InlineData("GroupId")]
        [InlineData("CreateDate")]
        public void UserPermissionHistoryHasExpectedFieldDefined(string name)
        {
            var sut = new UserPermissionHistory();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateUserPermissionId()
        {
            var items = faker.Generate(2);
            items[0].UserPermissionId = items[1].UserPermissionId;
            Assert.Equal(items[1].UserPermissionId, items[0].UserPermissionId);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdatePermissionMapId()
        {
            var items = faker.Generate(2);
            items[0].PermissionMapId = items[1].PermissionMapId;
            Assert.Equal(items[1].PermissionMapId, items[0].PermissionMapId);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateKeyValue()
        {
            var items = faker.Generate(2);
            items[0].KeyValue = items[1].KeyValue;
            Assert.Equal(items[1].KeyValue, items[0].KeyValue);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateKeyName()
        {
            var items = faker.Generate(2);
            items[0].KeyName = items[1].KeyName;
            Assert.Equal(items[1].KeyName, items[0].KeyName);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateGroupId()
        {
            var items = faker.Generate(2);
            items[0].GroupId = items[1].GroupId;
            Assert.Equal(items[1].GroupId, items[0].GroupId);
        }

        [Fact]
        public void UserPermissionHistoryCanUpdateCreateDate()
        {
            var items = faker.Generate(2);
            items[0].CreateDate = items[1].CreateDate;
            Assert.Equal(items[1].CreateDate, items[0].CreateDate);
        }

        [Theory]
        [InlineData("Id", "123-456-789")]
        [InlineData("UserPermissionId", "404-55-111")]
        [InlineData("UserId", "987-55-111")]
        [InlineData("PermissionMapId", "abcd-efg-hijk")]
        [InlineData("KeyValue", "party-all-the-time")]
        [InlineData("KeyName", "do-you-party")]
        [InlineData("GroupId", 1525)]
        public void UserPermissionHistoryCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new UserPermissionHistory();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}