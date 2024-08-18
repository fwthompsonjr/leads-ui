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
            lock (_locking)
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
                        ResourceFileService.Expire();
                        Assert.NotEmpty(ResourceFileService.Models);
                    });
                    Assert.Null(error);
                }
                finally
                {
                    ResourceFileService.Clear();
                } 
            }
        }

        [Fact]
        public void ServiceShouldNotFindMissingKey()
        {
            lock (_locking)
            {
                var error = Record.Exception(() =>
                    {
                        var model = faker.Generate();
                        var keyName = model.Name;
                        var actual = ResourceFileService.Get(keyName);
                        Assert.True(string.IsNullOrEmpty(actual));
                        ResourceFileService.Expire();
                    });
                Assert.Null(error); 
            }
        }

        [Fact]
        public void ServiceCanExpire()
        {
            lock (_locking)
            {
                try
                {
                    var error = Record.Exception(() =>
                            {
                                var models = faker.Generate(10);
                                var expiration = TimeSpan.FromSeconds(-60);
                                models.ForEach(m =>
                                {
                                    var keyName = m.Name;
                                    var keyValue = m.Content;
                                    ResourceFileService.AddOrUpdate(keyName, keyValue, expiration);
                                    var actual = ResourceFileService.Get(keyName);
                                    Assert.Equal(keyValue, actual);
                                });
                                Assert.Equal(10, ResourceFileService.Models.Count);
                                ResourceFileService.Expire();
                                Assert.Empty(ResourceFileService.Models);
                            });
                    Assert.Null(error);
                }
                finally
                {
                    ResourceFileService.Clear();
                }
            }
        }

        private static readonly object _locking = new();
    }

}
