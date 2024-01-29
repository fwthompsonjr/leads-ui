using legallead.jdbc.interfaces;
using legallead.search.api.Services;
using legallead.search.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace legallead.search.api.tests.Services
{
    public class SearchGenerationServiceTest
    {
        [Fact]
        public void ServiceCanBeConstructed()
        {
            var exception = Record.Exception(() =>
            {
                var provider = GetProvdier();
                var logger = provider.GetRequiredService<ILoggingRepository>();
                var repo = provider.GetRequiredService<ISearchQueueRepository>();
                var component = provider.GetRequiredService<IBgComponentRepository>();
                var settings = provider.GetRequiredService<IBackgroundServiceSettings>();
                var gen = provider.GetRequiredService<IExcelGenerator>();
                _ = new SearchGenerationService(logger, repo, component, settings, gen);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceIntergationSmokeTest()
        {
            if (!Debugger.IsAttached) return;
            var exception = Record.Exception(() =>
            {
                var provider = GetProvdier();
                var logger = provider.GetRequiredService<ILoggingRepository>();
                var repo = provider.GetRequiredService<ISearchQueueRepository>();
                var component = provider.GetRequiredService<IBgComponentRepository>();
                var settings = provider.GetRequiredService<IBackgroundServiceSettings>();
                var gen = provider.GetRequiredService<IExcelGenerator>();
                var service = new DemoGenerationService(logger, repo, component, settings, gen);
                service.TestWork();
            });
            Assert.Null(exception);
        }


        private static IServiceProvider GetProvdier()
        {
            var collection = new ServiceCollection();
            collection.Initialize();
            return collection.BuildServiceProvider();
        }

        private class DemoGenerationService : SearchGenerationService
        {
            public DemoGenerationService(ILoggingRepository? logger, ISearchQueueRepository? repo, IBgComponentRepository? component, IBackgroundServiceSettings? settings, IExcelGenerator excel) : base(logger, repo, component, settings, excel)
            {
            }

            public void TestWork()
            {
                DoWork(null);
            }
        }
    }
}
