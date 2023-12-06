using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.json.db;
using legallead.json.db.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace permissions.api.tests
{
    public class RegisterDataServicesTests
    {
        private static readonly object locker = new();
        private static IServiceProvider? _serviceProvider = null;

        public RegisterDataServicesTests()
        {
            lock (locker)
            {
                if (_serviceProvider != null) return;
                const string environmentName = "Development";
                var config =
                    new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{environmentName}.json", true)
                    .AddEnvironmentVariables()
                    .Build();
                var collection = new ServiceCollection();
                collection.AddSingleton<IConfiguration>(config);
                collection.RegisterAuthentication(config);
                collection.RegisterDataServices();
                _serviceProvider = collection.BuildServiceProvider(); 
            }
        }

        [Theory]
        [InlineData(typeof(IDataInitializer))]
        [InlineData(typeof(IJwtManagerRepository))]
        [InlineData(typeof(IDapperCommand))]
        [InlineData(typeof(DataContext))]
        [InlineData(typeof(IComponentRepository))]
        [InlineData(typeof(IPermissionMapRepository))]
        [InlineData(typeof(IProfileMapRepository))]
        [InlineData(typeof(IUserPermissionRepository))]
        [InlineData(typeof(IUserProfileRepository))]
        [InlineData(typeof(IUserTokenRepository))]
        [InlineData(typeof(IUserProfileViewRepository))]
        [InlineData(typeof(IUserPermissionViewRepository))]
        [InlineData(typeof(IPermissionGroupRepository))]
        [InlineData(typeof(IUserRepository))]
        [InlineData(typeof(DataProvider))]
        [InlineData(typeof(AccountController))]
        [InlineData(typeof(ApplicationController))]
        [InlineData(typeof(JsonInitStartupTask))]
        [InlineData(typeof(JdbcInitStartUpTask))]
        public void ProviderCanConstructInstance(Type type)
        {
            var exception = Record.Exception(() => {
                Assert.NotNull(_serviceProvider);
                _serviceProvider.GetService(type);
            });
            Assert.Null(exception);
        }
    }
}