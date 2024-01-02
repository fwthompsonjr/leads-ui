using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class ChangePermissionLevelRequestTests
    {
        private static readonly Faker<PermissionChoice> faker =
            new Faker<PermissionChoice>()
            .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .RuleFor(x => x.Level, y => y.Company.CompanyName());

        private readonly Faker<ChangePermissionLevelRequest> choiceFaker =
            new Faker<ChangePermissionLevelRequest>()
            .RuleFor(x => x.Choices, y => faker.Generate(y.Random.Int(1, 5)));

        [Fact]
        public void RequestCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = choiceFaker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionChoiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PermissionChoice();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionChoiceCanUpdateIsSelected()
        {
            var items = faker.Generate(2);
            items[0].IsSelected = items[1].IsSelected;
            Assert.Equal(items[1].IsSelected, items[0].IsSelected);
        }

        [Fact]
        public void PermissionChoiceCanUpdateLevel()
        {
            var items = faker.Generate(2);
            items[0].Level = items[1].Level;
            Assert.Equal(items[1].Level, items[0].Level);
        }

        [Fact]
        public void RequestCanUpdateChoices()
        {
            var items = choiceFaker.Generate(2);
            items[0].Choices = items[1].Choices;
            Assert.Equal(items[1].Choices, items[0].Choices);
        }
    }
}