using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.logging.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                collection.RegisterDataServices(config);
                collection.AddSingleton<IInternalServiceProvider>(new InternalServiceProvider(collection));
                collection.RegisterHealthChecks();
                _serviceProvider = collection.BuildServiceProvider();
            }
        }

        [Theory]
        [InlineData(typeof(IDataInitializer))]
        [InlineData(typeof(IDapperCommand))]
        [InlineData(typeof(DataContext))]
        [InlineData(typeof(IJwtManagerRepository))]
        [InlineData(typeof(IComponentRepository))]
        [InlineData(typeof(IPermissionMapRepository))]
        [InlineData(typeof(IProfileMapRepository))]
        [InlineData(typeof(IUserPermissionRepository))]
        [InlineData(typeof(IUserProfileRepository))]
        [InlineData(typeof(IUserTokenRepository))]
        [InlineData(typeof(IUserProfileViewRepository))]
        [InlineData(typeof(IUserPermissionViewRepository))]
        [InlineData(typeof(IPermissionGroupRepository))]
        [InlineData(typeof(IUserPermissionHistoryRepository))]
        [InlineData(typeof(IUserRepository))]
        [InlineData(typeof(DataProvider))]
        [InlineData(typeof(JsonInitStartupTask))]
        [InlineData(typeof(JdbcInitStartUpTask))]
        [InlineData(typeof(ISubscriptionInfrastructure))]
        [InlineData(typeof(IProfileInfrastructure))]
        [InlineData(typeof(ILogConfiguration))]
        [InlineData(typeof(ILoggingInfrastructure))]
        [InlineData(typeof(UserSearchValidator))]
        [InlineData(typeof(PermissionsController))]
        [InlineData(typeof(ApplicationController))]
        [InlineData(typeof(SignonController))]
        [InlineData(typeof(ListsController))]
        [InlineData(typeof(HomeController))]
        [InlineData(typeof(SearchController))]
        [InlineData(typeof(PaymentController))]
        public void ProviderCanConstructInstance(Type type)
        {
            var exception = Record.Exception(() =>
            {
                Assert.NotNull(_serviceProvider);
                _serviceProvider.GetService(type);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ProviderCanCreateMapper()
        {
            var mapper = ModelMapper.Mapper;
            Assert.NotNull(mapper);
        }
    }
}