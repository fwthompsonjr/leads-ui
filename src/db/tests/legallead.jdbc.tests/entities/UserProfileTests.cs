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
    }
}