using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests
{
    public class RegisterDataServicesTests
    {
        private readonly IServiceProvider _serviceProvider;
        public RegisterDataServicesTests()
        {
            var collection = new ServiceCollection();
            collection.RegisterDataServices();
            _serviceProvider = collection.BuildServiceProvider();
        }

        [Theory]
        [InlineData(typeof(IDapperCommand))]
        [InlineData(typeof(DataContext))]
        [InlineData(typeof(IComponentRepository))]
        [InlineData(typeof(IUserRepository))]
        [InlineData(typeof(DataProvider))]
        [InlineData(typeof(ApplicationController))]
        public void ProviderCanConstructInstance(Type type)
        {
            var exception = Record.Exception(() => _serviceProvider.GetService(type));
            Assert.Null(exception);
        }
    }
}
