using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace permissions.api.tests.Contollers
{
    public class DiscountCheckoutIntegrationTest
    {
        [Fact]
        public void CanGetHomeController()
        {
            var provider = GetProvider();
            var controller = provider.GetService<HomeController>();
            Assert.NotNull(controller);
        }

        [Theory]
        [InlineData("rBfFCeSJr89udeNN", "sub_1PLx7BDhgP60CL9xzZXF6rem")]
        public async Task VerifyCheckoutAsync(string id, string sessionId)
        {
            if (!Debugger.IsAttached) return;
            var errored = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<HomeController>();
                _ = await controller.DiscountCheckoutAsync(id, sessionId);
            });
            Assert.Null(errored);
        }

        private static IServiceProvider GetProvider()
        {
            if (_provider != null) return _provider;
            var services = new ServiceCollection();
            var config = GetConfiguration();
            services.RegisterAuthentication(config);
            services.RegisterDataServices(config);
            _provider = services.BuildServiceProvider();
            return _provider;
        }

        private static IConfiguration GetConfiguration()
        {
            const string environmentName = "Development";
            var config =
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
            return config;
        }

        private static IServiceProvider? _provider;
    }
}
