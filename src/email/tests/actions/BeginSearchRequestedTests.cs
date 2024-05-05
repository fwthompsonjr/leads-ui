using legallead.email.actions;
using legallead.email.interfaces;
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
    public class BeginSearchRequestedTests : IDisposable
    {
        public BeginSearchRequestedTests()
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
                var controller = provider.GetService<BeginSearchRequested>();
                Assert.NotNull(controller);
                controller.OnResultExecuting(executedContext);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        public void ProviderCanGetResultExecutedFiler(string payload)
        {
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var context = GetContext(payload);
                if (context is not ResultExecutedContext executedContext) return;
                var svc = provider.GetService<Mock<IUserSettingInfrastructure>>();
                var controller = provider.GetService<BeginSearchRequested>();
                Assert.NotNull(controller);
                controller.OnResultExecuted(executedContext);
            });
            Assert.Null(exception);
        }

        private static readonly object locker = new();

        private static FilterContext GetContext(string result, bool isExecuted = true)
        {

            var collection = new ServiceCollection();
            collection.AddTransient<BeginSearchRequested>();
            collection.AddMvcCore(o =>
            {
                o.Filters.AddService<BeginSearchRequested>();
            });
            collection.AddTransient<MockController>();
            var provider = collection.BuildServiceProvider();
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
            return MockMessageInfrastructure.GetServiceProvider(true);
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
            [ServiceFilter(typeof(BeginSearchRequested))] // Apply the filter to this action method
            public IActionResult MyAction()
            {
                var guid = Guid.NewGuid().ToString();
                return Ok(guid);
            }
        }
    }
}