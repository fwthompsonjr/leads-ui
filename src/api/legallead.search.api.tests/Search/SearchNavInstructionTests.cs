using Bogus;
using legallead.models.Search;

namespace legallead.search.api.tests.Search
{
    public class SearchNavInstructionTests
    {
        private static readonly Faker<SearchNavInstruction> faker =
            new Faker<SearchNavInstruction>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.FriendlyName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.By, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.CommandType, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.Value, y => y.Random.AlphaNumeric(50));

        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() => _ = new SearchNavInstruction());
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
        public void ModelCanGetSetFriendlyName()
        {
            var tests = faker.Generate(2);
            tests[0].FriendlyName = tests[1].FriendlyName;
            Assert.Equal(tests[0].FriendlyName, tests[1].FriendlyName);
        }

        [Fact]
        public void ModelCanGetSetBy()
        {
            var tests = faker.Generate(2);
            tests[0].By = tests[1].By;
            Assert.Equal(tests[0].By, tests[1].By);
        }

        [Fact]
        public void ModelCanGetSetValue()
        {
            var tests = faker.Generate(2);
            tests[0].Value = tests[1].Value;
            Assert.Equal(tests[0].Value, tests[1].Value);
        }

        [Fact]
        public void ModelCanGetSetCommandType()
        {
            var tests = faker.Generate(2);
            tests[0].CommandType = tests[1].CommandType;
            Assert.Equal(tests[0].CommandType, tests[1].CommandType);
        }
    }
}