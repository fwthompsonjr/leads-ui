using Bogus;
using legallead.records.search.Models;
using Xunit;

namespace legallead.records.search.tests.Models
{
    public class ResourceFileModelTests
    {
        private static readonly Faker<ResourceFileModel> faker
            = new Faker<ResourceFileModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Content, y => y.Lorem.Paragraphs(4))
            .RuleFor(x => x.ExpirationDate, y => y.Date.Recent(120));

        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new ResourceFileModel();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(error);
        }
    }
}
