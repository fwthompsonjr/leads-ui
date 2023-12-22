using legallead.logging.helpers;
using legallead.logging.interfaces;

namespace legallead.logging.tests
{
    public class LoggingDbServiceProviderTests
    {
        [Fact]
        public void ProviderCanBeCreated()
        {
            var provider = new LoggingDbServiceProvider();
            Assert.NotNull(provider);
        }

        [Fact]
        public void ProviderCanGetService()
        {
            var provider = new LoggingDbServiceProvider();
            Assert.NotNull(provider);
            Assert.NotNull(provider.Provider);
        }

        [Theory]
        [InlineData(typeof(ILogConfiguration))]
        [InlineData(typeof(ILoggingDbCommand))]
        [InlineData(typeof(ILoggingDbContext))]
        [InlineData(typeof(ILogContentRepository))]
        [InlineData(typeof(ILoggingService))]
        public void ProviderCanGetInstance(Type type)
        {
            var provider = new LoggingDbServiceProvider().Provider;
            var instance = provider.GetService(type);
            Assert.NotNull(instance);
        }
    }
}