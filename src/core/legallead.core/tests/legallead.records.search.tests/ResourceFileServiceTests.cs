using Bogus;
using legallead.records.search.Models;
using Xunit;

namespace legallead.records.search.tests
{
    public class ResourceFileServiceTests
    {
        private static readonly Faker<ResourceFileModel> faker
            = new Faker<ResourceFileModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Content, y => y.Lorem.Paragraphs(4))
            .RuleFor(x => x.ExpirationDate, y => y.Date.Recent(120));

        [Fact]
        public void ServiceContainsModels()
        {
            var error = Record.Exception(() =>
            {
                _ = ResourceFileService.Models;
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanAddOrUpdate()
        {
            try
            {
                var error = Record.Exception(() =>
                {
                    var models = faker.Generate(2);
                    var keyName = models[0].Name;
                    var expiration = TimeSpan.FromSeconds(60);
                    models.ForEach(m =>
                    {
                        var keyValue = m.Content;
                        ResourceFileService.AddOrUpdate(keyName, keyValue, expiration);
                        var actual = ResourceFileService.Get(keyName);
                        Assert.Equal(keyValue, actual);
                    });
                });
                Assert.Null(error);
            }
            finally
            {
                ResourceFileService.Clear();
            }
        }

        [Fact]
        public void ServiceShouldNotFindMissingKey()
        {
            var error = Record.Exception(() =>
            {
                var model = faker.Generate();
                var keyName = model.Name;
                var actual = ResourceFileService.Get(keyName);
                Assert.True(string.IsNullOrEmpty(actual));
            });
            Assert.Null(error);
        }
    }

}
