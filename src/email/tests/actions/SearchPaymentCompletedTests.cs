using Bogus;
using legallead.email.actions;
using legallead.email.interfaces;
using legallead.email.models;
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
    public class SearchPaymentCompletedTests : IDisposable
    {
        public SearchPaymentCompletedTests()
        {
            _ = InitializeProvider();
        }

        [Theory]
        [InlineData("not-a-guid")]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        public void ProviderCanGetResultExecutingFiler(string payload)
        {
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var context = GetContext(payload, false);
                if (context is not ResultExecutingContext executedContext) return;
                var controller = provider.GetService<SearchPaymentCompleted>();
                Assert.NotNull(controller);
                controller.OnResultExecuting(executedContext);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("not-a-guid")]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79")]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79", false)]
        [InlineData("2a03793a-7327-4a5a-af11-ddb3851f4b79", true, false)]
        public void ProviderCanGetResultExecutedFiler(string payload, bool hasAccount = true, bool hasAccountId = true)
        {
            var mfaker = new Faker();
            var exception = Record.Exception(() =>
            {
                var provider = InitializeProvider();
                var accountId = hasAccountId ? mfaker.Random.Guid().ToString() : null;
                var account = hasAccount ? new UserAccountByEmailBo
                {
                    Email = mfaker.Person.Email,
                    Id = accountId,
                    UserName = mfaker.Person.UserName
                } : null;
                var userDb = provider.GetRequiredService<Mock<IUserSettingInfrastructure>>();
                userDb.Setup(m => m.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(account);
                var context = GetContext(payload);
                if (context is not ResultExecutedContext executedContext) return;
                var controller = provider.GetService<SearchPaymentCompleted>();
                Assert.NotNull(controller);
                controller.OnResultExecuted(executedContext);
            });
            Assert.Null(exception);
        }

        private static readonly object locker = new();

        private static FilterContext GetContext(string result, bool isExecuted = true)
        {
            if (Guid.TryParse(result, out var _))
            {
                result = Properties.Resources.payment_completed_html;
            }
            var collection = new ServiceCollection();
            collection.AddTransient<SearchPaymentCompleted>();
            collection.AddMvcCore(o =>
            {
                o.Filters.AddService<SearchPaymentCompleted>();
            });
            var content = new ContentResult { Content = result };
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
                        content,
                        controller
                    );
            }
            else
            {
                return new ResultExecutingContext(
                    actionContext,
                        new List<IFilterMetadata>(),
                        content,
                        controller
                    );
            }
        }

        private static ServiceProvider InitializeProvider()
        {
            var provider = MockMessageInfrastructure.GetServiceProvider(true);
            var userDb = provider.GetRequiredService<Mock<IUserSettingInfrastructure>>();
            var settings = MockMessageInfrastructure.GetDefaultSettings();
            userDb.Setup(s => s.GetSettings(
                It.Is<UserSettingQuery>(q => IsGuid(q.Id)))).ReturnsAsync(settings);
            return provider;
        }
        private static bool IsGuid(string? id)
        {
            return Guid.TryParse(id ?? "", out var _);
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
            [ServiceFilter(typeof(SearchPaymentCompleted))] // Apply the filter to this action method
            public IActionResult MyAction()
            {
                var guid = Guid.NewGuid().ToString();
                return Ok(guid);
            }
        }
    }
}