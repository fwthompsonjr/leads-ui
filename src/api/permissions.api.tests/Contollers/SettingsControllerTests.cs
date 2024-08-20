using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace permissions.api.tests.Contollers
{
    public class SettingsControllerTests
    {
        private static readonly Faker<AppSettingRequest> requestfaker =
            new Faker<AppSettingRequest>()
                .RuleFor(x => x.KeyName, y => y.Random.AlphaNumeric(10));
        private static readonly Faker faker = new();

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SutCanGetFindKey(int responseId)
        {
            var error = Record.Exception(() =>
            {
                var response = responseId switch
                {
                    0 => null,
                    1 => "",
                    _ => faker.Random.AlphaNumeric(15)
                };
                var statusCode = responseId switch
                {
                    0 => 204,
                    1 => 204,
                    3 => 400,
                    _ => 200
                };
                var hasHeader = responseId != 3;
                var provider = GetProvider(hasHeader);
                var infra = provider.GetRequiredService<Mock<IAppSettingService>>();
                infra.Setup(s => s.FindKey(It.IsAny<string>())).Returns(response);
                var service = provider.GetRequiredService<SettingsController>();
                var actionResult = service.AppSetting(requestfaker.Generate());
                if (hasHeader) infra.Verify(s => s.FindKey(It.IsAny<string>()));
                if (actionResult is not IStatusCodeActionResult status) { return; }
                Assert.Equal(statusCode, status.StatusCode);
            });
            Assert.Null(error);
        }

        private static ServiceProvider GetProvider(bool hasHeader = true)
        {
            var service = new ServiceCollection();
            var appheader = ApplicationModel.GetApplicationsFallback().FirstOrDefault() ?? new();
            var mqSearch = new Mock<IAppSettingService>();
            var serialObj = JsonConvert.SerializeObject(appheader);
            var dictionary = new HeaderDictionary();
            if (hasHeader)
            {
                dictionary.Append("APP_IDENTITY", new Microsoft.Extensions.Primitives.StringValues(serialObj));
            }
            //Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            request.SetupGet(x => x.Headers).Returns(dictionary);
            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            service.AddSingleton(mqSearch);
            service.AddSingleton(mqSearch.Object);
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton(m =>
            {
                var controller = new SettingsController(mqSearch.Object)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            return service.BuildServiceProvider();
        }
    }
}