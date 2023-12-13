using legallead.content.helpers;
using legallead.content.interfaces;

namespace legallead.content.tests
{
    public class ContentDbServiceProviderTests
    {
        [Fact]
        public void ProviderCanBeCreated()
        {
            var provider = new ContentDbServiceProvider();
            Assert.NotNull(provider);
        }

        [Fact]
        public void ProviderCanGetService()
        {
            var provider = new ContentDbServiceProvider();
            Assert.NotNull(provider);
            Assert.NotNull(provider.Provider);
        }

        [Theory]
        [InlineData(typeof(IContentDbCommand))]
        [InlineData(typeof(ContentDbContext))]
        public void ProviderCanGetInstance(Type type)
        {
            var provider = new ContentDbServiceProvider().Provider;
            var instance = provider.GetService(type);
            Assert.NotNull(instance);
        }
    }
}