using Bogus;
using legallead.desktop.entities;

namespace legallead.desktop.tests.entities
{
    public class SubscriptionChoiceTests
    {
        private readonly Faker<SubscriptionChoice> faker =
            new Faker<SubscriptionChoice>()
            .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .RuleFor(x => x.Level, y => y.Company.CompanyName());

        [Fact]
        public void SubscriptionChoiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SubscriptionChoice();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SubscriptionChoiceCanUpdateIsSelected()
        {
            var items = faker.Generate(2);
            items[0].IsSelected = items[1].IsSelected;
            Assert.Equal(items[1].IsSelected, items[0].IsSelected);
        }

        [Fact]
        public void SubscriptionChoiceCanUpdateLevel()
        {
            var items = faker.Generate(2);
            items[0].Level = items[1].Level;
            Assert.Equal(items[1].Level, items[0].Level);
        }
    }
}