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
    }
}