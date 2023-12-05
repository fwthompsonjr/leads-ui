using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserProfileViewTests
    {

        private readonly Faker<UserProfileView> faker =
            new Faker<UserProfileView>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ProfileMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.OrderId, y => y.Random.Int(5, 25055));

        [Fact]
        public void UserProfileViewCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserProfileView();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserProfileViewIsBaseDto()
        {
            var sut = new UserProfileView();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Fact]
        public void UserProfileViewHasTableNameDefined()
        {
            var expected = "vwuserprofile";
            var sut = new UserProfileView();
            Assert.Equal(expected, sut.TableName);
        }

        [Fact]
        public void UserProfileViewHasFieldListDefined()
        {
            var expected = new[] { "Id", "UserId", "ProfileMapId", "KeyValue", "KeyName", "OrderId" };
            var sut = new UserProfileView();
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
        [InlineData("KeyName")]
        [InlineData("OrderId")]
        public void UserProfileViewHasExpectedFieldDefined(string name)
        {
            var sut = new UserProfileView();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Fact]
        public void UserProfileViewCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UserProfileViewCanUpdateUserId()
        {
            var items = faker.Generate(2);
            items[0].UserId = items[1].UserId;
            Assert.Equal(items[1].UserId, items[0].UserId);
        }

        [Fact]
        public void UserProfileViewCanUpdateProfileMapId()
        {
            var items = faker.Generate(2);
            items[0].ProfileMapId = items[1].ProfileMapId;
            Assert.Equal(items[1].ProfileMapId, items[0].ProfileMapId);
        }

        [Fact]
        public void UserProfileViewCanUpdateKeyValue()
        {
            var items = faker.Generate(2);
            items[0].KeyValue = items[1].KeyValue;
            Assert.Equal(items[1].KeyValue, items[0].KeyValue);
        }

        [Fact]
        public void UserProfileViewCanUpdateKeyName()
        {
            var items = faker.Generate(2);
            items[0].KeyName = items[1].KeyName;
            Assert.Equal(items[1].KeyName, items[0].KeyName);
        }

        [Fact]
        public void UserProfileViewCanUpdateOrderId()
        {
            var items = faker.Generate(2);
            items[0].OrderId = items[1].OrderId;
            Assert.Equal(items[1].OrderId, items[0].OrderId);
        }

        [Theory]
        [InlineData("Id", "123-456-789")]
        [InlineData("UserId", "987-55-111")]
        [InlineData("ProfileMapId", "abcd-efg-hijk")]
        [InlineData("KeyValue", "party-all-the-time")]
        [InlineData("KeyName", "do-you-party")]
        [InlineData("OrderId", 1525)]
        public void UserProfileViewCanReadWriteByIndex(string fieldName, object expected)
        {
            var sut = new UserProfileView();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = expected;
            var actual = sut[position];
            Assert.Equal(expected, actual);
        }
    }
}