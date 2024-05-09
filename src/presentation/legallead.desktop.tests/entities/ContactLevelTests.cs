using Bogus;
using legallead.desktop.entities;

namespace legallead.desktop.tests.entities
{
    public class ContactLevelTests
    {
        private static readonly Faker<ContactLevel> faker
            = new Faker<ContactLevel>()
            .RuleFor(x => x.Level, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new ContactLevel();
            var test = faker.Generate();
            Assert.NotEqual(original.Level, test.Level);
        }
    }
}