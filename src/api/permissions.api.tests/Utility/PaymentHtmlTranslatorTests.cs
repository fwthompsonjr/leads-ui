using AutoMapper.Internal;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Stripe;

namespace permissions.api.tests.Utility
{
    public class PaymentHtmlTranslatorTests
    {
        [Theory]
        [InlineData(typeof(Mock<IUserSearchRepository>))]
        [InlineData(typeof(Mock<ISubscriptionInfrastructure>))]
        [InlineData(typeof(Mock<ICustomerInfrastructure>))]
        [InlineData(typeof(Mock<IUserRepository>))]
        [InlineData(typeof(IUserSearchRepository))]
        [InlineData(typeof(ISubscriptionInfrastructure))]
        [InlineData(typeof(IStripeInfrastructure))]
        [InlineData(typeof(ICustomerInfrastructure))]
        [InlineData(typeof(Mock<SubscriptionService>))]
        [InlineData(typeof(IUserRepository))]
        [InlineData(typeof(StripeKeyEntity))]
        [InlineData(typeof(PaymentHtmlTranslator))]
        public void ProviderHasRegisteredType(Type target)
        {
            var provider = GetProvider();
            var obj = provider.GetService(target);
            Assert.NotNull(obj);

        }
        [Fact]
        public void BuilderCanBeCreated()
        {
            var exception = Record.Exception(() => { 
                using var builder = new PaymentHtmlTranslatorBuilder();
                // check properties
                _ = builder.Provider;
                _ = builder.Translator;
                _ = builder.Repo;
                _ = builder.SubscriptionDb;
                _ = builder.CustDb;
                _ = builder.UserDb;
                _ = builder.MockRepo;
                _ = builder.MockSubscriptionDb;
                _ = builder.MockCustDb;
                _ = builder.MockUserDb;
                _ = builder.MockSubscriptionService;
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SutCanGetSubscriptionService(bool clearObject)
        {
            var error = Record.Exception(() =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                if (clearObject) builder.Translator.SetupSubscriptionService(null);
                var service = builder.Translator.GetSubscriptionService;
                Assert.NotNull(service);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("success", "1234567", true)]
        [InlineData("cancel", "1234567", true)]
        [InlineData("success", null, true)]
        [InlineData("success", "", true)]
        [InlineData(null, "1234567", true)]
        [InlineData("unmapped", "1234567", true)]
        [InlineData("success", "1234567", false)]
        public async Task SutCanValidateRequest(string? status, string? id, bool isValid)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                var apiResponse = requestNames.Contains(status) && isValid;
                var repo = builder.MockRepo;
                var service = builder.Translator;
                repo.Setup(m => m.IsValidExternalId(It.IsAny<string>())).ReturnsAsync(apiResponse);
                _ = await service.IsRequestValid(status, id);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("1234567", true)]
        [InlineData(null, true)]
        [InlineData("   ", true)]
        [InlineData("1234567", false)]
        public async Task SutCanValidateSession(string? id, bool isValid)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                var apiResponse = isValid ? sessionFaker.Generate() : null;
                var repo = builder.MockRepo;
                var service = builder.Translator;
                repo.Setup(m => m.GetPaymentSession(It.IsAny<string>())).ReturnsAsync(apiResponse);
                _ = await service.IsSessionValid(id);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true, null)]
        [InlineData(true, true, "")]
        [InlineData(true, true, "NONE")]
        [InlineData(true, true, "abc", false)]
        public async Task SutCanValidateSubscription(
            bool hasRequest,
            bool hasSubscription,
            string? invoiceUri = "abc",
            bool hasSessionId = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                var apiResponse = hasRequest ? levelFaker.Generate() : null;
                var svcResponse = hasSubscription ? new Subscription() : null;
                if (apiResponse != null && invoiceUri != "abc") apiResponse.InvoiceUri = invoiceUri;
                if (apiResponse != null && !hasSessionId) apiResponse.SessionId = null;
                var repo = builder.MockSubscriptionDb;
                var subSvc = builder.MockSubscriptionService;
                var service = builder.Translator;

                repo.Setup(m => m.GetLevelRequestById(
                    It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(apiResponse);
                subSvc.Setup(m => m.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<SubscriptionGetOptions>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(svcResponse);
                _ = await service.IsSubscriptionValid("1234", "abc-def");
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true, null)]
        [InlineData(true, true, "")]
        [InlineData(true, true, "NONE")]
        [InlineData(true, true, "abc", false)]
        public async Task SutCanValidateDiscount(
            bool hasRequest,
            bool hasSubscription,
            string? invoiceUri = "abc",
            bool hasSessionId = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                var apiResponse = hasRequest ? levelFaker.Generate() : null;
                var svcResponse = hasSubscription ? new Subscription() : null;
                if (apiResponse != null && invoiceUri != "abc") apiResponse.InvoiceUri = invoiceUri;
                if (apiResponse != null && !hasSessionId) apiResponse.SessionId = null;
                var repo = builder.MockSubscriptionDb;
                var subSvc = builder.MockSubscriptionService;
                var service = builder.Translator;

                repo.Setup(m => m.GetDiscountRequestById(
                    It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(apiResponse);
                subSvc.Setup(m => m.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<SubscriptionGetOptions>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(svcResponse);
                _ = await service.IsDiscountValid("1234", "abc-def");
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(false, true, true, true, false, true)] // happy path
        [InlineData(true, true, true, true, false, true)] // happy path
        [InlineData(false, false, true, true, false, true)]
        [InlineData(false, true, false, true, false, true)]
        [InlineData(false, true, true, false, false, true)]
        [InlineData(false, true, true, true, true, true)]
        [InlineData(false, true, true, true, false, false)]
        [InlineData(false, true, true, true, false, true, null)]
        [InlineData(false, true, true, true, false, true, "")]
        public async Task SutCanValidateIsRequestPaid(
            bool isDtoNull,
            bool dtoHasJson,
            bool dtoJsonIsValid,
            bool dtoJsonHasData,
            bool dtoJsonHasEmptyData,
            bool? searchPurchaseResponse,
            string? referenceId = "abc")
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                var dto = isDtoNull ? null : sessionFaker.Generate();
                if (dto != null && !dtoHasJson) dto.JsText = null;
                if (dto != null && !dtoJsonIsValid) dto.JsText = "abcdefghijklmnop";
                if (dto != null && !dtoJsonHasData)
                {
                    var payment = paymentfaker.Generate();
                    payment.Data = new();
                    dto.JsText = JsonConvert.SerializeObject(payment);
                }
                if (dto != null && !dtoJsonHasEmptyData)
                {
                    var payment = paymentfaker.Generate();
                    payment.Data.Clear();
                    dto.JsText = JsonConvert.SerializeObject(payment);
                }
                if (dto != null && referenceId != "abc" && !string.IsNullOrEmpty(dto.JsText))
                {
                    var payment = JsonConvert.DeserializeObject<PaymentSessionJs>(dto.JsText) ?? new();
                    if (payment.Data.Any())
                    {
                        payment.Data[0].ReferenceId = referenceId;
                    }
                    dto.JsText = JsonConvert.SerializeObject(payment);
                }
                var repo = builder.MockRepo;
                var service = builder.Translator;

                repo.Setup(m => m.IsSearchPurchased(
                    It.IsAny<string>())).ReturnsAsync(searchPurchaseResponse);

                _ = await service.IsRequestPaid(dto);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true, null)]
        [InlineData(true, true, "")]
        [InlineData(true, true, "NONE")]
        [InlineData(true, true, "abc", false)]
        [InlineData(true, true, "abc", true, "incomplete")]
        [InlineData(true, true, "abc", true, "incomplete_expired")]
        [InlineData(true, true, "abc", true, "canceled")]
        [InlineData(true, true, "abc", false, "cleared", false)]
        public async Task SutCanValidateIsLevelRequestPaid(
            bool hasRequest,
            bool hasSubscription,
            string? invoiceUri = "abc",
            bool hasSessionId = true,
            string? sessionStatus = "good",
            bool isPaymentSuccess = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                LevelRequestBo apiResponse = hasRequest ? levelFaker.Generate() : new();
                var svcResponse = hasSubscription ? new Subscription { Status = sessionStatus } : null;
                if (invoiceUri != "abc") apiResponse.InvoiceUri = invoiceUri;
                if (!hasSessionId) apiResponse.SessionId = null;
                if (!isPaymentSuccess) apiResponse.InvoiceUri = "NONE";

                var subSvc = builder.MockSubscriptionService;
                var service = builder.Translator;
                subSvc.Setup(m => m.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<SubscriptionGetOptions>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(svcResponse);
                _ = await service.IsRequestPaid(apiResponse);
                _ = await service.IsDiscountPaid(apiResponse);
            });
            Assert.Null(error);
        }
        private static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            var repo = new Mock<IUserSearchRepository>();
            var subscriptionDb = new Mock<ISubscriptionInfrastructure>();
            var custDb = new Mock<ICustomerInfrastructure>();
            var userDb = new Mock<IUserRepository>();
            var stripeConfig = new Mock<StripeKeyEntity>(); 
            var stripe = new Mock<IStripeInfrastructure>();
            var subscription = new Mock<SubscriptionService>();
            var paymentKey = keyFaker.Generate();
            stripeConfig.Setup(x => x.GetActiveName()).Returns(paymentKey.PaymentKey);
            services.AddSingleton(repo);
            services.AddSingleton(stripe);
            services.AddSingleton(subscriptionDb);
            services.AddSingleton(subscription);
            services.AddSingleton(custDb);
            services.AddSingleton(userDb);
            services.AddSingleton(stripeConfig.Object);
            services.AddSingleton(stripe.Object);
            services.AddSingleton(repo.Object);
            services.AddSingleton(subscriptionDb.Object);
            services.AddSingleton(subscription.Object);
            services.AddSingleton(custDb.Object);
            services.AddSingleton(userDb.Object);
            services.AddSingleton(x =>
            {
                var repo = x.GetRequiredService<IUserSearchRepository>();
                var subscriptionDb = x.GetRequiredService<ISubscriptionInfrastructure>();
                var custDb = x.GetRequiredService<ICustomerInfrastructure>();
                var userDb = x.GetRequiredService<IUserRepository>();
                var stripeConfig = x.GetRequiredService<StripeKeyEntity>();
                var stripe = x.GetRequiredService<IStripeInfrastructure>();
                var translator = new PaymentHtmlTranslator(repo, userDb, custDb, subscriptionDb, stripe, stripeConfig);
                var subscription = x.GetRequiredService<Mock<SubscriptionService>>();
                translator.SetupSubscriptionService(subscription.Object);
                return translator;
            });
            return services.BuildServiceProvider();
        }

        private sealed class PaymentKeyWrapper
        {
            public string PaymentKey { get; set; } = string.Empty;
        }

        private static readonly Faker<PaymentKeyWrapper> keyFaker
            = new Faker<PaymentKeyWrapper>()
            .RuleFor(x => x.PaymentKey, y => y.Random.AlphaNumeric(8));


        private static readonly Faker<SearchInvoiceBo> invoiceFaker
            = new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 5))
            .RuleFor(x => x.UnitPrice, y => y.Random.Decimal(1, 10))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .FinishWith((a, b) =>
            {
                b.Price = b.UnitPrice.GetValueOrDefault() * b.ItemCount.GetValueOrDefault();
            });


        private static readonly Faker<PaymentSessionJs> paymentfaker = new Faker<PaymentSessionJs>()
            .RuleFor(x => x.Description, y => y.Person.UserName)
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SuccessUrl, y => y.Random.Guid().ToString())
            .FinishWith((a, b) =>
            {
                var invoice = invoiceFaker.Generate(a.Random.Int(2, 6));
                b.Data = invoice;
            });
        private static readonly Faker<PaymentSessionDto> sessionFaker
            = new Faker<PaymentSessionDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.UserId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.InvoiceId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.SessionType, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.SessionId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.IntentId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.ClientId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.ExternalId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.JsText, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .FinishWith((a, b) =>
            {
                var data = paymentfaker.Generate();
                b.JsText = JsonConvert.SerializeObject(data);
            });

        private static readonly Faker<LevelRequestBo> levelFaker
            = new Faker<LevelRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.UserId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.InvoiceUri, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.LevelName, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.SessionId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
        private static readonly string[] requestNames = new[] { "success", "cancel" };

        private sealed class PaymentHtmlTranslatorBuilder : IDisposable
        {
            private readonly IServiceProvider _provider;
            private readonly IPaymentHtmlTranslator _translator;
            private readonly IUserSearchRepository _repo;
            private readonly ISubscriptionInfrastructure _subscriptionDb;
            private readonly ICustomerInfrastructure _custDb;
            private readonly IUserRepository _userDb;
            private readonly Mock<IUserSearchRepository> _mockrepo;
            private readonly Mock<ISubscriptionInfrastructure> _mocksubscriptionDb;
            private readonly Mock<ICustomerInfrastructure> _mockcustDb;
            private readonly Mock<IUserRepository> _mockuserDb;
            private readonly Mock<SubscriptionService> _subscriptionService;
            private bool disposedValue;

            public PaymentHtmlTranslatorBuilder()
            {
                _provider = GetProvider();
                _translator = _provider.GetRequiredService<PaymentHtmlTranslator>();
                _repo = _provider.GetRequiredService<IUserSearchRepository>();
                _subscriptionDb = _provider.GetRequiredService<ISubscriptionInfrastructure>();
                _custDb = _provider.GetRequiredService<ICustomerInfrastructure>();
                _userDb = _provider.GetRequiredService<IUserRepository>();
                _mockrepo = _provider.GetRequiredService<Mock<IUserSearchRepository>>();
                _mocksubscriptionDb = _provider.GetRequiredService<Mock<ISubscriptionInfrastructure>>();
                _mockcustDb = _provider.GetRequiredService<Mock<ICustomerInfrastructure>>();
                _mockuserDb = _provider.GetRequiredService<Mock<IUserRepository>>();
                _subscriptionService = _provider.GetRequiredService<Mock<SubscriptionService>>();
            }

            public IServiceProvider Provider => _provider;
            public IPaymentHtmlTranslator Translator => _translator;
            public IUserSearchRepository Repo => _repo;
            public ISubscriptionInfrastructure SubscriptionDb => _subscriptionDb;
            public ICustomerInfrastructure CustDb => _custDb;
            public IUserRepository UserDb => _userDb;
            public Mock<IUserSearchRepository> MockRepo => _mockrepo;
            public Mock<ISubscriptionInfrastructure> MockSubscriptionDb => _mocksubscriptionDb;
            public Mock<ICustomerInfrastructure> MockCustDb => _mockcustDb;
            public Mock<IUserRepository> MockUserDb => _mockuserDb;
            public Mock<SubscriptionService> MockSubscriptionService => _subscriptionService;

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        InvoiceExtensions.GetInfrastructure = null;
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
}
