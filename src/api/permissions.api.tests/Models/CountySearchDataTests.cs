using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class CountySearchDataTests
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

        private static readonly Faker<CountySearchDetail> detailfaker =
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
			

        private static readonly Faker<CountySearchData> faker =
            new Faker<CountySearchData>()
            .RuleFor(x => x.Data, y => detailfaker.Generate());

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CountySearchData();
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
        public void ModelCanGetCaseSearchTypes()
        {
            var test = faker.Generate();
            Assert.NotNull(test.Data);
        }

        [Fact]
        public void ModelCanSetData()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Data = source.Data;
            Assert.Equal(source.Data, test.Data);
        }
    }
}