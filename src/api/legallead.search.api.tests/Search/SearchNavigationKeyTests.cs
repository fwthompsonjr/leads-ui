using Bogus;
using legallead.models.Search;

namespace legallead.search.api.tests.Search
{
    public class SearchNavigationKeyTests
    {
        private static readonly Faker<SearchNavigationKey> faker =
            new Faker<SearchNavigationKey>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.Value, y => y.Random.AlphaNumeric(50));

        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() => _ = new SearchNavigationKey());
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() => _ = faker.Generate());
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetSetName()
        {
            var tests = faker.Generate(2);
            tests[0].Name = tests[1].Name;
            Assert.Equal(tests[0].Name, tests[1].Name);
        }

        [Fact]
        public void ModelCanGetSetValue()
        {
            var tests = faker.Generate(2);
            tests[0].Value = tests[1].Value;
            Assert.Equal(tests[0].Value, tests[1].Value);
        }
    }
}
