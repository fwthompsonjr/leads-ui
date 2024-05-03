using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Utility;
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
            var exception = Record.Exception(() =>
            {
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
                var apiResponse = PaymentHtmlHelper.requestNames.Contains(status) && isValid;
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
                var apiResponse = isValid ? PaymentHtmlHelper.SessionFaker.Generate() : null;
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
                var apiResponse = hasRequest ? PaymentHtmlHelper.LevelFaker.Generate() : null;
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
                var apiResponse = hasRequest ? PaymentHtmlHelper.LevelFaker.Generate() : null;
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
                var dto = isDtoNull ? null : PaymentHtmlHelper.SessionFaker.Generate();
                if (dto != null && !dtoHasJson) dto.JsText = null;
                if (dto != null && !dtoJsonIsValid) dto.JsText = "abcdefghijklmnop";
                if (dto != null && !dtoJsonHasData)
                {
                    var payment = PaymentHtmlHelper.PaymentFaker.Generate();
                    payment.Data = new();
                    dto.JsText = JsonConvert.SerializeObject(payment);
                }
                if (dto != null && !dtoJsonHasEmptyData)
                {
                    var payment = PaymentHtmlHelper.PaymentFaker.Generate();
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
        [InlineData(false, true, true, true, false, true)] // happy path
        [InlineData(true, true, true, true, false, true)]
        [InlineData(false, false, true, true, false, true)]
        [InlineData(false, true, false, true, false, true)]
        [InlineData(false, true, true, false, false, true)]
        [InlineData(false, true, true, true, true, true)]
        [InlineData(false, true, true, true, false, false)]
        [InlineData(false, true, true, true, false, true, null)]
        [InlineData(false, true, true, true, false, true, "")]
        [InlineData(false, true, true, true, false, true, "abc", null)]
        [InlineData(false, true, true, true, false, true, "abc", false)]
        public async Task SutCanValidateIsRequestPaidAndDownloaded(
            bool isDtoNull,
            bool dtoHasJson,
            bool dtoJsonIsValid,
            bool dtoJsonHasData,
            bool dtoJsonHasEmptyData,
            bool? searchIsPaid = true,
            string? referenceId = "abc",
            bool? searchIsDownloaded = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                var dto = isDtoNull ? null : PaymentHtmlHelper.SessionFaker.Generate();
                if (dto != null && !dtoHasJson) dto.JsText = null;
                if (dto != null && !dtoJsonIsValid) dto.JsText = "abcdefghijklmnop";
                if (dto != null && !dtoJsonHasData)
                {
                    var payment = PaymentHtmlHelper.PaymentFaker.Generate();
                    payment.Data = new();
                    dto.JsText = JsonConvert.SerializeObject(payment);
                }
                if (dto != null && !dtoJsonHasEmptyData)
                {
                    var payment = PaymentHtmlHelper.PaymentFaker.Generate();
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
                var serviceResponse = PaymentHtmlHelper.GenerateSearchIsPaid(searchIsPaid, searchIsDownloaded);
                repo.Setup(m => m.IsSearchPaidAndDownloaded(
                    It.IsAny<string>())).ReturnsAsync(serviceResponse);

                _ = await service.IsRequestDownloadedAndPaid(dto);
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
                LevelRequestBo apiResponse = hasRequest ? PaymentHtmlHelper.LevelFaker.Generate() : new();
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true, true)]
        [InlineData(true, true, true, false, false)]
        public async Task SutCanExecuteGetDownload(
            bool sessionHasData = true,
            bool hasReferenceId = true,
            bool hasRecords = true,
            bool createDownloadError = false,
            bool sessionHasJson = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                PaymentSessionDto request = PaymentHtmlHelper.SessionFaker.Generate();
                var rc = new Faker().Random.Int(5, 15);
                var response = hasRecords ? PaymentHtmlHelper.SearchFinalFaker.Generate(rc) : new();
                var subSvc = builder.MockRepo;
                var service = builder.Translator;
                if (!sessionHasData)
                {
                    var js = request.JsText ?? string.Empty;
                    var payment = JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
                    payment.Data.Clear();
                    request.JsText = JsonConvert.SerializeObject(payment);
                }
                if (!hasReferenceId)
                {
                    var js = request.JsText ?? string.Empty;
                    var payment = JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
                    payment.Data[0].ReferenceId = null;
                    request.JsText = JsonConvert.SerializeObject(payment);
                }
                if (createDownloadError)
                {
                    var exception = new Faker().System.Exception();
                    subSvc.Setup(m => m.CreateOrUpdateDownloadRecord(
                        It.IsAny<string>(),
                        It.IsAny<string>())).ThrowsAsync(exception);
                }
                if (!sessionHasJson)
                {
                    request.JsText = string.Empty;
                }
                subSvc.Setup(m => m.GetFinal(It.IsAny<string>())).ReturnsAsync(response);
                _ = await service.GetDownload(request);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("success", "12345", true)]
        [InlineData("cancel", "12345", true)]
        [InlineData("success", "", true)]
        [InlineData("success", null, true)]
        [InlineData("missing", "12345", true)]
        [InlineData("success", "12345", false)]
        [InlineData("success", "12345", true, "empty")]
        public async Task SutCanExecuteIsDiscountLevel(
            string? status,
            string? id,
            bool hasDiscount,
            string? discountId = "abc")
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                LevelRequestBo? apiResponse = hasDiscount ? PaymentHtmlHelper.LevelFaker.Generate() : null;
                if (discountId != "abc" && apiResponse != null) apiResponse.Id = discountId;
                var svc = builder.MockCustDb;
                var service = builder.Translator;
                svc.Setup(m => m.GetDiscountRequestById(
                    It.IsAny<string>())).ReturnsAsync(apiResponse);
                _ = await service.IsDiscountLevel(status, id);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("success", "12345", true)]
        [InlineData("cancel", "12345", true)]
        [InlineData("success", "", true)]
        [InlineData("success", null, true)]
        [InlineData("missing", "12345", true)]
        [InlineData("success", "12345", false)]
        [InlineData("success", "12345", true, "empty")]
        public async Task SutCanExecuteIsChangeUserLevel(
            string? status,
            string? id,
            bool hasDiscount,
            string? discountId = "abc")
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                LevelRequestBo? apiResponse = hasDiscount ? PaymentHtmlHelper.LevelFaker.Generate() : null;
                if (discountId != "abc" && apiResponse != null) apiResponse.Id = discountId;
                var svc = builder.MockCustDb;
                var service = builder.Translator;
                svc.Setup(m => m.GetLevelRequestById(
                    It.IsAny<string>())).ReturnsAsync(apiResponse);
                _ = await service.IsChangeUserLevel(status, id);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("base", "base")]
        [InlineData("base", null)]
        [InlineData("base", "")]
        [InlineData(null, "base")]
        [InlineData("", "base")]
        [InlineData("base", "base", false)]
        public async Task SutCanResetDownload(
            string? userId,
            string? externalId,
            bool hasResponse = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                DownloadHistoryDto? apiResponse = hasResponse ? new() : null;
                var payload = PaymentHtmlHelper.DownloadRequestFaker.Generate();
                if (string.IsNullOrEmpty(userId)) { payload.UserId = userId; }
                if (string.IsNullOrEmpty(externalId)) { payload.ExternalId = externalId; }
                var svc = builder.MockRepo;
                var service = builder.Translator;
                svc.Setup(m => m.AllowDownloadRollback(
                    It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(apiResponse);
                _ = await service.ResetDownload(payload);
            });
            Assert.Null(error);
        }

        private static IServiceProvider GetProvider()
        {
            return PaymentHtmlHelper.GetProvider();
        }



    }
}
