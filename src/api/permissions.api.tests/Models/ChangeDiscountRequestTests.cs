using legallead.models;
using legallead.permissions.api.Extensions;

namespace permissions.api.tests.Models
{
    using UsState = legallead.json.db.entity.UsState;
    using UsStateCounty = legallead.json.db.entity.UsStateCounty;
    public class ChangeDiscountRequestTests
    {
        private static readonly object locker = new();

        private readonly Faker<ChangeDiscountRequest> choiceFaker =
            new Faker<ChangeDiscountRequest>()
            .RuleFor(x => x.Choices, y => faker.Generate(y.Random.Int(1, 5)));

        private static readonly Faker<DiscountChoice> faker =
            new Faker<DiscountChoice>()
            .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .RuleFor(x => x.StateName, y => y.Company.CompanyName())
            .RuleFor(x => x.CountyName, y => y.Company.CompanyName());

        public ChangeDiscountRequestTests()
        {
            lock (locker)
            {
                UsState.Initialize();
                UsStateCounty.Initialize();
            }
        }

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
        public void DiscountChoiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DiscountChoice();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DiscountChoiceCanUpdateIsSelected()
        {
            var items = faker.Generate(2);
            items[0].IsSelected = items[1].IsSelected;
            Assert.Equal(items[1].IsSelected, items[0].IsSelected);
        }

        [Fact]
        public void DiscountChoiceCanUpdateCountyName()
        {
            var items = faker.Generate(2);
            items[0].CountyName = items[1].CountyName;
            Assert.Equal(items[1].CountyName, items[0].CountyName);
        }

        [Fact]
        public void DiscountChoiceCanUpdateStateName()
        {
            var items = faker.Generate(2);
            items[0].StateName = items[1].StateName;
            Assert.Equal(items[1].StateName, items[0].StateName);
        }

        [Fact]
        public void DiscountRequestCanUpdateChoices()
        {
            var items = choiceFaker.Generate(2);
            items[0].Choices = items[1].Choices;
            Assert.Equal(items[1].Choices, items[0].Choices);
        }

        [Theory]
        [InlineData("TX", true)]
        [InlineData("texas", true)]
        [InlineData("not-a-state-name", false)]
        [InlineData("CA", true)]
        [InlineData("", false)]
        public void DiscountChoiceCanConvertToState(string stateName, bool expected)
        {
            var item = faker.Generate();
            item.StateName = stateName;
            var converted = item.ToState();
            if (expected) Assert.NotNull(converted);
            else Assert.Null(converted);
        }

        [Theory]
        [InlineData("TX", "Collin", true)]
        [InlineData("texas", "Denton", true)]
        [InlineData("TX", "not-a-name", false)]
        [InlineData("CA", "Collin", false)]
        public void DiscountChoiceCanConvertToCounty(string stateName, string county, bool expected)
        {
            var item = faker.Generate();
            item.StateName = stateName;
            item.CountyName = county;
            var converted = item.ToCounty();
            if (expected) Assert.NotNull(converted);
            else Assert.Null(converted);
        }
    }
}