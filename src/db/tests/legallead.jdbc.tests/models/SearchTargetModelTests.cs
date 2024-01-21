using Bogus;
using legallead.jdbc.models;

namespace legallead.jdbc.tests.models
{
    public class SearchTargetModelTests
    {
        private static readonly Faker<SearchTargetModel> faker =
            new Faker<SearchTargetModel>()
            .RuleFor(x => x.Component, y => y.Company.CompanyName())
            .RuleFor(x => x.SearchId, y => y.Random.AlphaNumeric(12))
            .RuleFor(x => x.LineNbr, y => y.Random.Int())
            .RuleFor(x => x.Line, y => y.Random.AlphaNumeric(28))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchTargetModel();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanReadWrite()
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                a.Component = b.Component;
                a.SearchId = b.SearchId;
                a.LineNbr = b.LineNbr;
                a.Line = b.Line;
                a.CreateDate = b.CreateDate;
            });
            Assert.Null(exception);
        }
    }
}