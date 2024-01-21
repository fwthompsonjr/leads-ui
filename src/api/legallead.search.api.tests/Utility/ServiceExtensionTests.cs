using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.logging.interfaces;
using legallead.search.api.Controllers;
using legallead.search.api.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.search.api.tests.Utility
{
    public class ServiceExtensionTests
    {
        [Fact]
        public void AppCanBeBuilt()
        {
            var exception = Record.Exception(() =>
            {
                _ = GetWeb();
            });
            Assert.Null(exception);
        }
        [Fact]
        public void ProviderCanBeBuilt()
        {
            var exception = Record.Exception(() =>
            {
                _ = GetProvider();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(typeof(DataContext))]
        [InlineData(typeof(ApiController))]
        [InlineData(typeof(ILogConfiguration))]
        [InlineData(typeof(ILoggingService))]
        [InlineData(typeof(ILoggingRepository))]
        [InlineData(typeof(ISearchQueueRepository))]
        [InlineData(typeof(IBgComponentRepository))]
        [InlineData(typeof(IBackgroundServiceSettings))]
        [InlineData(typeof(IExcelGenerator))]
        public void ProviderCanRetrieveType(Type target)
        {
            var provider = GetProvider();
            var obj = provider.GetService(target);
            Assert.NotNull(obj);
        }

        private static IServiceProvider GetProvider()
        {
            var collection = new ServiceCollection();
            collection.Initialize();
            return collection.BuildServiceProvider();
        }

        private static WebApplication GetWeb()
        {
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            services.Initialize();
            var app = builder.Build();
            app.Initialize();
            return app;
        }
    }
}
