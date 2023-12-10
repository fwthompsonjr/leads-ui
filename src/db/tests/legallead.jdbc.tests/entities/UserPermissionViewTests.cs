using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserPermissionViewTests
    {
        private readonly Faker<UserPermissionView> faker =
            new Faker<UserPermissionView>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.OrderId, y => y.Random.Int(5, 25055));

        [Fact]
        public void UserPermissionViewCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserPermissionView();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserPermissionViewIsBaseDto()
        {
            var sut = new UserPermissionView();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserPermissionViewHasTableNameDefined()
        {
            var expected = "vwuserpermission";
            var sut = new UserPermissionView();
            Assert.Equal(expected, sut.TableName, true);
        }

        [Fact]
        public void UserPermissionViewHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserId", "PermissionMapId", "KeyValue", "KeyName", "OrderId" };
            var sut = new UserPermissionView();
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
        [InlineData("KeyName")]
        [InlineData("OrderId")]
        public void UserPermissionViewHasExpectedFieldDefined(string name)
        {
            var sut = new UserPermissionView();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserPermissionViewCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserPermissionViewCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserPermissionViewCanUpdatePermissionMapId()
        {
            var items = faker.Generate(2);
            items[0].PermissionMapId = items[1].PermissionMapId;
            Assert.Equal(items[1].PermissionMapId, items[0].PermissionMapId);
        }

        [Fact]
        public void UserPermissionViewCanUpdateKeyValue()
        {
            var items = faker.Generate(2);
            items[0].KeyValue = items[1].KeyValue;
            Assert.Equal(items[1].KeyValue, items[0].KeyValue);
        }

        [Fact]
        public void UserPermissionViewCanUpdateKeyName()
        {
            var items = faker.Generate(2);
            items[0].KeyName = items[1].KeyName;
            Assert.Equal(items[1].KeyName, items[0].KeyName);
        }

        [Fact]
        public void UserPermissionViewCanUpdateOrderId()
        {
            var items = faker.Generate(2);
            items[0].OrderId = items[1].OrderId;
            Assert.Equal(items[1].OrderId, items[0].OrderId);
        }

        [Theory]
        [InlineData("Id", "123-456-789")]
        [InlineData("UserId", "987-55-111")]
        [InlineData("PermissionMapId", "abcd-efg-hijk")]
        [InlineData("KeyValue", "party-all-the-time")]
        [InlineData("KeyName", "do-you-party")]
        [InlineData("OrderId", 1525)]
        public void UserPermissionViewCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new UserPermissionView();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}