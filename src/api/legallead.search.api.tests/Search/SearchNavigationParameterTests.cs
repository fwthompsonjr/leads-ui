using Bogus;
using legallead.models.Search;

namespace legallead.search.api.tests.Search
{
    public class SearchNavigationParameterTests
    {

        private static readonly Faker<SearchNavigationKey> keyfaker =
            new Faker<SearchNavigationKey>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.Value, y => y.Random.AlphaNumeric(50));

        private static readonly Faker<SearchNavInstruction> navfaker =
            new Faker<SearchNavInstruction>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.FriendlyName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.By, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.CommandType, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.Value, y => y.Random.AlphaNumeric(50));


        private static readonly Faker<SearchNavigationParameter> faker =
            new Faker<SearchNavigationParameter>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 100000))
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.Keys, y => { var nn = y.Random.Int(1, 5); return keyfaker.Generate(nn); })
            .RuleFor(x => x.Instructions, y => { var nn = y.Random.Int(1, 5); return navfaker.Generate(nn); })
            .RuleFor(x => x.CaseInstructions, y => { var nn = y.Random.Int(1, 5); return navfaker.Generate(nn); });

        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() => _ = new SearchNavigationParameter());
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() => _ = faker.Generate());
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetSetId()
        {
            var tests = faker.Generate(2);
            tests[0].Id = tests[1].Id;
            Assert.Equal(tests[0].Id, tests[1].Id);
        }

        [Fact]
        public void ModelCanGetSetName()
        {
            var tests = faker.Generate(2);
            tests[0].Name = tests[1].Name;
            Assert.Equal(tests[0].Name, tests[1].Name);
        }

        [Fact]
        public void ModelCanGetSetStartDate()
        {
            var tests = faker.Generate(2);
            tests[0].StartDate = tests[1].StartDate;
            Assert.Equal(tests[0].StartDate, tests[1].StartDate);
        }

        [Fact]
        public void ModelCanGetSetEndDate()
        {
            var tests = faker.Generate(2);
            tests[0].EndDate = tests[1].EndDate;
            Assert.Equal(tests[0].EndDate, tests[1].EndDate);
        }

        [Fact]
        public void ModelCanGetSetKeys()
        {
            var tests = faker.Generate(2);
            tests[0].Keys = tests[1].Keys;
            Assert.Equal(tests[0].Keys, tests[1].Keys);
        }

        [Fact]
        public void ModelCanGetSetInstructions()
        {
            var tests = faker.Generate(2);
            tests[0].Instructions = tests[1].Instructions;
            Assert.Equal(tests[0].Instructions, tests[1].Instructions);
        }

        [Fact]
        public void ModelCanGetSetCaseInstructions()
        {
            var tests = faker.Generate(2);
            tests[0].CaseInstructions = tests[1].CaseInstructions;
            Assert.Equal(tests[0].CaseInstructions, tests[1].CaseInstructions);
        }
    }
}