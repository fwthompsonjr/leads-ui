using Bogus;
using legallead.email.actions;
using legallead.email.implementations;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.services;
using legallead.email.transforms;
using legallead.email.utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.email.tests.actions
{
    public class AccountRegistrationCompletedTests : IDisposable
    {
        public AccountRegistrationCompletedTests()
        {
            _ = InitializeProvider();
        }

        [Fact]
        public void ControllerCanBeCreated()
        {
            var provider = InitializeProvider();
            var controller = provider.GetRequiredService<MockController>();
            Assert.NotNull(controller);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        public void ProviderCanGetResultExecutingFiler(string payload)
        {
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var context = GetContext(payload, false);
                if (context is not ResultExecutingContext executedContext) return;
                var controller = provider.GetService<AccountRegistrationCompleted>();
                Assert.NotNull(controller);
                controller.OnResultExecuting(executedContext);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        public void ProviderCanGetResultExecutedFiler(string payload)
        {
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var context = GetContext(payload);
                if (context is not ResultExecutedContext executedContext) return;
                var controller = provider.GetService<AccountRegistrationCompleted>();
                Assert.NotNull(controller);
                controller.OnResultExecuted(executedContext);
            });
            Assert.Null(exception);
        }
        [Fact]
        public void ControllerCanExecuteMyAction()
        {
            var provider = InitializeProvider();
            var controller = provider.GetRequiredService<MockController>();
            var result = controller.MyAction();
            Assert.NotNull(result);
        }
        [ApiController]
        private sealed class MockController : ControllerBase
        {
            [HttpGet]
            [ServiceFilter(typeof(AccountRegistrationCompleted))] // Apply the filter to this action method
            public IActionResult MyAction()
            {
                var guid = Guid.NewGuid().ToString();
                return Ok(guid);
            }
        }
        private static readonly object locker = new();

        private static FilterContext GetContext(string result, bool isExecuted = true)
        {
            var provider = InitializeProvider();
            var controller = provider.GetRequiredService<MockController>();
            // Create a default ActionContext (depending on our case-scenario)
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            if (isExecuted)
            {
                return new ResultExecutedContext(
                    actionContext,
                        new List<IFilterMetadata>(),
                        new OkObjectResult(result),
                        controller
                    );
            }
            else
            {
                return new ResultExecutingContext(
                    actionContext,
                        new List<IFilterMetadata>(),
                        new OkObjectResult(result),
                        controller
                    );
            }
        }

        private static ServiceProvider InitializeProvider()
        {
            lock (locker)
            {
                var services = new ServiceCollection();
                var connectionMock = new Mock<IConnectionStringService>();
                var cryptoMock = new Mock<ICryptographyService>();
                var dbMock = new Mock<IDataCommandService>();
                var dbConnectionMock = new Mock<IDataConnectionService>();
                var smtpWrapperMock = new Mock<ISmtpClientWrapper>();
                var smtpMock = new Mock<ISmtpService>();
                var userDbMock = new Mock<IUserSettingInfrastructure>();
                var collection = GetDefaultSettings();
                userDbMock.Setup(s => s.GetSettings(It.IsAny<UserSettingQuery>())).ReturnsAsync(collection);
                // add mocks
                services.AddSingleton(collection);
                services.AddSingleton(connectionMock);
                services.AddSingleton(cryptoMock);
                services.AddSingleton(dbMock);
                services.AddSingleton(dbConnectionMock);
                services.AddSingleton(smtpWrapperMock);
                services.AddSingleton(smtpMock);
                services.AddSingleton(userDbMock);
                // add implementations
                services.AddSingleton(connectionMock.Object);
                services.AddSingleton(cryptoMock.Object);
                services.AddSingleton(dbMock.Object);
                services.AddSingleton(dbConnectionMock.Object);
                services.AddSingleton(smtpWrapperMock.Object);
                services.AddSingleton(smtpMock.Object);
                services.AddSingleton(userDbMock.Object);
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddTransient<IHtmlTransformService, HtmlTransformService>();
                services.AddKeyedTransient<IHtmlTransformDetailBase, AccountRegistrationTemplate>("AccountRegistration");
                services.AddTransient<AccountRegistrationCompleted>();
                services.AddTransient(x =>
                {
                    var settings = x.GetRequiredService<ISettingsService>();
                    var infra = userDbMock.Object;
                    var transform = x.GetRequiredService<IHtmlTransformService>();
                    return new MailMessageService(settings, infra, transform);
                });
                services.AddMvcCore(options =>
                {
                    options.Filters.AddService<AccountRegistrationCompleted>();
                });
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
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                };
                services.AddScoped(a =>
                {
                    return new MockController()
                    {
                        ControllerContext = controllerContext
                    };
                });

                var provider = services.BuildServiceProvider();
                ServiceInfrastructure.Provider = provider;
                return provider;
            }
        }

        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            var list = new List<UserEmailSettingBo>
            {
                new ()
            };
            commonKeys.ForEach(k =>
            {
                var item = faker.Generate();
                item.KeyName = k;
                if (k == "Person") item.KeyValue = new Faker().Person.FullName;
                list.Add(item);
            });
            var template = list[1];
            list.ForEach(a =>
            {
                a.Id = template.Id;
                a.Email = template.Email;
                a.UserName = template.UserName;
            });
            return list;
        }


        private static readonly List<string> commonKeys =
        [
            "Email 1",
            "Email 2",
            "Email 3",
            "First Name",
            "Last Name"
        ];

        private static readonly Faker<UserEmailSettingBo> faker =
            new Faker<UserEmailSettingBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.KeyName, y => y.PickRandom(commonKeys))
            .FinishWith((a, b) =>
            {
                b.KeyValue = b.KeyName switch
                {
                    "First Name" => a.Person.FirstName,
                    "Last Name" => a.Person.LastName,
                    _ => b.KeyValue
                };
            });
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (locker)
                    {
                        ServiceInfrastructure.Provider = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
