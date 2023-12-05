using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests
{
    public class RegisterDataServicesTests
    {
        private readonly IServiceProvider _serviceProvider;

        public RegisterDataServicesTests()
        {
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

        [Theory]
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
        [InlineData(typeof(IUserRepository))]
        [InlineData(typeof(DataProvider))]
        [InlineData(typeof(AccountController))]
        [InlineData(typeof(ApplicationController))]
        public void ProviderCanConstructInstance(Type type)
        {
            var exception = Record.Exception(() => _serviceProvider.GetService(type));
            Assert.Null(exception);
        }
    }
}