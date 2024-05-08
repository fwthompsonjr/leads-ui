using legallead.email.actions;
using legallead.email.interfaces;
using legallead.email.utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.tests.actions
{
    public class PermissionChangeRequestedTests : IDisposable
    {
        public PermissionChangeRequestedTests()
        {
            _ = InitializeProvider();
        }

        [Theory]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        public void ProviderCanGetResultExecutingFiler(string payload)
        {
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var context = GetContext(payload, false);
                if (context is not ResultExecutingContext executedContext) return;
                var controller = provider.GetService<PermissionChangeRequested>();
                Assert.NotNull(controller);
                controller.OnResultExecuting(executedContext);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        [InlineData("29e8ffcc-1a5d-4ea5-824d-e9555e3161d8")]
        [InlineData("1a8ee09e-2c57-4055-adbd-458cd41125ea")]
        [InlineData("f5dbdb76-4be8-43b7-ad75-2df8c4864078")]
        [InlineData("c216278c-2bf3-4625-b4d2-807859fd663f")]
        public void ProviderCanGetResultExecutedFiler(string payload)
        {
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var context = GetContext(payload);
                if (context is not ResultExecutedContext executedContext) return;
                var controller = provider.GetService<PermissionChangeRequested>();
                var svc = provider.GetRequiredService<Mock<IUserSettingInfrastructure>>();
                var user = MessageMockInfrastructure.UserAccountFaker.Generate();
                svc.Setup(m => m.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
                Assert.NotNull(controller);
                controller.OnResultExecuted(executedContext);
            });
            Assert.Null(exception);
        }

        private static readonly object locker = new();

        private static FilterContext GetContext(string result, bool isExecuted = true)
        {
            var collection = new ServiceCollection();
            collection.AddTransient<PermissionChangeRequested>();
            collection.AddMvcCore(o =>
            {
                o.Filters.AddService<PermissionChangeRequested>();
            });
            collection.AddTransient<MockController>();
            var provider = collection.BuildServiceProvider();
            var controller = provider.GetRequiredService<MockController>();
            // Create a default ActionContext (depending on our case-scenario)
            var name = PermissionMockInfrastructure.GetChangeType();
            var ok = string.IsNullOrWhiteSpace(result) ?
                PermissionMockInfrastructure.GetResult(404, "None") :
                PermissionMockInfrastructure.GetResult(200, name);
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
                        ok,
                        controller
                    );
            }
            else
            {
                return new ResultExecutingContext(
                    actionContext,
                        new List<IFilterMetadata>(),
                        ok,
                        controller
                    );
            }
        }

        private static ServiceProvider InitializeProvider()
        {
            return MessageMockInfrastructure.GetServiceProvider(true);
        }

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

        [ApiController]
        private sealed class MockController : ControllerBase
        {
            [HttpGet]
            [ServiceFilter(typeof(PermissionChangeRequested))] // Apply the filter to this action method
            public IActionResult MyAction()
            {
                var guid = Guid.NewGuid().ToString();
                return Ok(guid);
            }
        }
    }
}