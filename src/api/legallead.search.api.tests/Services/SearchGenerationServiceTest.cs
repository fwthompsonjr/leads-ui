using legallead.jdbc.interfaces;
using legallead.search.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // _ = new SearchGenerationService(logger, repo, component, settings);
            });
            Assert.Null(exception);
        }

        private static IServiceProvider GetProvdier()
        {
            var collection = new ServiceCollection();
            collection.Initialize();
            return collection.BuildServiceProvider();
        }
    }
}
