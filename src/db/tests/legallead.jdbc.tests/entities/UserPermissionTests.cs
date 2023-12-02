using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserPermissionTests
    {
        private readonly Faker<UserPermission> faker =
            new Faker<UserPermission>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Company.CompanyName());

        [Fact]
        public void UserPermissionCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserPermission();
            });
            Assert.Null(exception);
        }


        [Fact]
        public void UserPermissionIsBaseDto()
        {
            var sut = new UserPermission();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserPermissionHasTableNameDefined()
        {
            var expected = "userpermission";
            var sut = new UserPermission();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void UserPermissionHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserId", "PermissionMapId", "KeyValue" };
            var sut = new UserPermission();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Equal(expected.Length, fields.Count);
        }


        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("PermissionMapId")]
        [InlineData("KeyValue")]
        public void UserPermissionHasExpectedFieldDefined(string name)
        {
            var sut = new UserPermission();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserPermissionCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserPermissionCanUpdatePermissionMapId()
        {
            var items = faker.Generate(2);
            items[0].PermissionMapId = items[1].PermissionMapId;
            Assert.Equal(items[1].PermissionMapId, items[0].PermissionMapId);
        }

        [Fact]
        public void UserPermissionCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserPermissionCanUpdateKeyValue()
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
        public void UserPermissionCanReadWriteByIndex(int position, object expected)
        {
            var sut = new UserPermission();
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}