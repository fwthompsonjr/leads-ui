using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class ComponentTests
    {
        private readonly Faker<Component> faker =
            new Faker<Component>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        [Fact]
        public void ComponentCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new Component();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ComponentCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ComponentCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }
    }
}