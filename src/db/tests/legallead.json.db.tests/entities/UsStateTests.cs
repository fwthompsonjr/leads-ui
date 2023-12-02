using Bogus;
using legallead.json.db.entity;

namespace legallead.json.db.tests.entities
{
    public class UsStateTests
    {
        private readonly Faker<UsState> faker =
            new Faker<UsState>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ShortName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsActive, y => y.Random.Bool());

        [Fact]
        public void UsStateCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UsState();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UsStateCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UsStateCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void UsStateCanUpdateShortName()
        {
            var items = faker.Generate(2);
            items[0].ShortName = items[1].ShortName;
            Assert.Equal(items[1].ShortName, items[0].ShortName);
        }

        [Fact]
        public void UsStateCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }
    }
}