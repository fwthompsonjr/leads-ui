using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class CountySearchDetailTests
    {
        private static readonly Faker<CaseSearchType> searchFaker =
            new Faker<CaseSearchType>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<DropDownOption> ddFaker =
            new Faker<DropDownOption>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<CboDropDown> cbofaker =
            new Faker<CboDropDown>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Members, y =>
            {
                var count = y.Random.Int(1, 10);
                return ddFaker.Generate(count);
            })
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<CountySearchDetail> faker =
            new Faker<CountySearchDetail>()
            .RuleFor(x => x.DropDowns, y =>
            {
                var count = y.Random.Int(1, 10);
                return cbofaker.Generate(count);
            })
            .RuleFor(x => x.CaseSearchTypes, y =>
            {
                var count = y.Random.Int(1, 10);
                return searchFaker.Generate(count);
            });

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CountySearchDetail();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGetDropDowns()
        {
            var test = faker.Generate();
            Assert.NotEmpty(test.DropDowns);
        }

        [Fact]
        public void ModelCanSetDropDowns()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.DropDowns = source.DropDowns;
            Assert.Equal(source.DropDowns, test.DropDowns);
        }

        [Fact]
        public void ModelCanGetCaseSearchTypes()
        {
            var test = faker.Generate();
            Assert.NotNull(test.CaseSearchTypes);
            Assert.NotEmpty(test.CaseSearchTypes);
        }

        [Fact]
        public void ModelCanSetCaseSearchTypes()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.CaseSearchTypes = source.CaseSearchTypes;
            Assert.Equal(source.CaseSearchTypes, test.CaseSearchTypes);
        }
    }
}