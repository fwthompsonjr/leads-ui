using Bogus;
using legallead.json.db.entity;

namespace legallead.json.db.tests.entities
{
    public class UsStateCountyTests
    {
        private readonly Faker<UsStateCounty> faker =
            new Faker<UsStateCounty>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Index, y => y.Random.Int(10000, 90000))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ShortName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsActive, y => y.Random.Bool());

        [Fact]
        public void UsStateCountyCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UsStateCounty();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UsStateCountyCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void UsStateCountyCanUpdateIndex()
        {
            var items = faker.Generate(2);
            items[0].Index = items[1].Index;
            Assert.Equal(items[1].Index, items[0].Index);
        }

        [Fact]
        public void UsStateCountyCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void UsStateCountyCanUpdateStateCode()
        {
            var items = faker.Generate(2);
            items[0].StateCode = items[1].StateCode;
            Assert.Equal(items[1].StateCode, items[0].StateCode);
        }

        [Fact]
        public void UsStateCountyCanUpdateShortName()
        {
            var items = faker.Generate(2);
            items[0].ShortName = items[1].ShortName;
            Assert.Equal(items[1].ShortName, items[0].ShortName);
        }

        [Fact]
        public void UsStateCountyCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }
    }
}