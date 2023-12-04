using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public abstract class BaseControllerTest
    {

        private static IServiceProvider? _serviceProvider;
        protected static IServiceProvider GetProvider()
        {
            if (_serviceProvider != null) { return _serviceProvider; }

            //Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );

            //Controller needs a controller context 
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var compMk = new Mock<IComponentRepository>();
            var userMk = new Mock<IUserRepository>();
            var collection = new ServiceCollection();
            collection.AddScoped(s => request);
            collection.AddScoped(s => userMk);
            collection.AddScoped(s => compMk);
            collection.AddScoped(s => userMk.Object);
            collection.AddScoped(s => compMk.Object);
            collection.AddScoped(d =>
            {
                var a = d.GetRequiredService<IComponentRepository>();
                var b = d.GetRequiredService<IUserRepository>();
                return new DataProvider(a, b);
            });
            collection.AddScoped(a =>
            {
                var db = a.GetRequiredService<DataProvider>();
                return new ApplicationController(db)
                {
                    ControllerContext = controllerContext
                };
            });
            _serviceProvider = collection.BuildServiceProvider();
            return _serviceProvider;

        }
    }
}
