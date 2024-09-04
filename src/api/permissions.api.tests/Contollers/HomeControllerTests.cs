using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace permissions.api.tests.Contollers
{
    using ApiResources = legallead.permissions.api.Properties.Resources;
    public class HomeControllerTests
    {
        [Fact]
        public void ControllerCanGetIndex()
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var controller = new HomeController(
                html.Object,
                infrastructure.Object,
                subscription.Object,
                lockdb.Object,
                stripeSvcs.Object,
                secretSvc.Object);
            var indx = controller.Index();
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetPaymentLandingAsync(bool isValid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var controller = new HomeController(
                html.Object,
                infrastructure.Object,
                subscription.Object,
                lockdb.Object,
                stripeSvcs.Object,
                secretSvc.Object);
            var content = GetPaymentLandingContent(isValid);
            html.Setup(m => m.IsRequestValidAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            html.Setup(m => m.TransformAsync(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(content).Verifiable(Times.Once);
            var indx = await controller.PaymentLandingAsync("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ControllerCanGetPaymentCheckoutAsync(bool isSessionValid, bool isRequestPaid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            PaymentSessionDto? session = isSessionValid ? new PaymentSessionDto() : null;
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            html.Setup(m => m.IsRequestValidAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            html.Setup(m => m.IsSessionValidAsync(It.IsAny<string>())).ReturnsAsync(session);
            html.Setup(m => m.IsRequestPaidAsync(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestPaid);
            var indx = await controller.PaymentCheckoutAsync("abcd");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetSubscriptionLandingAsync(bool isValid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            var content = GetUserLevelLandingContent(isValid);
            html.Setup(m => m.IsChangeUserLevelAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            html.Setup(m => m.TransformForPermissionsAsync(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(content).Verifiable(Times.Once);
            var indx = await controller.UserLevelLandingAsync("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ControllerCanGetSubscriptionCheckoutAsync(bool isSessionValid, bool isRequestPaid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            LevelRequestBo? session = isSessionValid ? new LevelRequestBo() : null;
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            html.Setup(m => m.IsChangeUserLevelAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            html.Setup(m => m.IsSubscriptionValidAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(session);
            html.Setup(m => m.IsRequestPaidAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(isRequestPaid);
            stripeSvcs.Setup(m => m.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync("123456789");
            var indx = await controller.SubscriptionCheckoutAsync("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ControllerCanGetDiscountCheckoutAsync(bool isSessionValid, bool isRequestPaid)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var html = new Mock<IPaymentHtmlTranslator>();
                var infrastructure = new Mock<ISearchInfrastructure>();
                var subscription = new Mock<ISubscriptionInfrastructure>();
                var lockdb = new Mock<ICustomerLockInfrastructure>();
                LevelRequestBo? session = isSessionValid ? new LevelRequestBo() : null;
                var stripeSvcs = new Mock<IStripeInfrastructure>();
                var secretSvc = new Mock<IClientSecretService>();
                var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
                html.Setup(m => m.IsDiscountLevelAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
                html.Setup(m => m.IsDiscountValidAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(session);
                html.Setup(m => m.IsDiscountPaidAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(isRequestPaid);
                _ = await controller.DiscountCheckoutAsync("abcd", "123");

            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetDiscountLandingAsync(bool isValid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            var content = GetUserLevelLandingContent(isValid);
            html.Setup(m => m.IsDiscountLevelAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            html.Setup(m => m.TransformForDiscountsAsync(It.IsAny<ISubscriptionInfrastructure>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(content).Verifiable(Times.Once);
            var indx = await controller.DiscountLandingAsync("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ControllerCanGetIntentLandingAsync(bool isValid, bool hasClientId)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var clientId = hasClientId ? "abcd" : string.Empty;
            PaymentSessionDto? dto = isValid ? new() { ClientId = clientId } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            html.Setup(m => m.IsSessionValidAsync(It.IsAny<string>())).ReturnsAsync(dto);
            var indx = await controller.FetchIntentAsync(new());
            Assert.NotNull(indx);
            if (dto == null || string.IsNullOrEmpty(dto.ClientId))
                Assert.IsType<ContentResult>(indx);
            else
                Assert.IsType<JsonResult>(indx);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ControllerCanFetchSubscriptionIntentAsync(bool isValid, bool hasSessionId)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var secret = new { Id = "00000" };
            var sessionId = hasSessionId ? "abcd" : string.Empty;
            LevelRequestBo? dto = isValid ? new() { SessionId = sessionId } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            subscription.Setup(m => m.GetLevelRequestByIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dto);
            stripeSvcs.Setup(m => m.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(secret.Id);
            stripeSvcs.Setup(m => m.FetchClientSecretAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(secret);
            var indx = await controller.FetchSubscriptionIntentAsync(new());
            Assert.NotNull(indx);
            Assert.IsType<JsonResult>(indx);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ControllerCanFetchDiscountIntentAsync(bool isValid, bool hasSessionId)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var secret = new { Id = "00000" };
            var sessionId = hasSessionId ? "abcd" : string.Empty;
            LevelRequestBo? dto = isValid ? new() { SessionId = sessionId } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);
            subscription.Setup(m => m.GetDiscountRequestByIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dto);
            stripeSvcs.Setup(m => m.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(secret.Id);
            stripeSvcs.Setup(m => m.FetchClientSecretAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(secret);
            var indx = await controller.FetchDiscountIntentAsync(new());
            Assert.NotNull(indx);
            Assert.IsType<JsonResult>(indx);
        }

        [Theory]
        [InlineData(true, true, false, true, false, true, false)]
        [InlineData(false, true, false, true, false, true, false)]
        [InlineData(true, false, false, true, false, true, false)]
        [InlineData(true, true, true, true, false, true, false)]
        [InlineData(true, true, false, true, false, false, false)]
        [InlineData(true, true, false, false, false, true, false)]
        [InlineData(true, true, false, true, true, true, false)]
        [InlineData(true, true, false, true, false, true, true)]
        public async Task ControllerCanFetchDownloadAsync(
            bool hasUser,
            bool hasUserId,
            bool isAccountLocked,
            bool isSessionValid,
            bool isSessionEmpty,
            bool isRequestPaid,
            bool isRequestDownloadedAndPaid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var userId = hasUserId ? "1234567" : string.Empty;
            User? user = hasUser ? new User { Id = userId } : null;
            var down = new DownloadResponse();
            var sessionJs = isSessionEmpty ? string.Empty : "abcd";
            PaymentSessionDto? dto = isSessionValid ? new() { Id = "1223456", JsText = sessionJs } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);

            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            lockdb.Setup(m => m.IsAccountLockedAsync(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            html.Setup(m => m.IsSessionValidAsync(It.IsAny<string>())).ReturnsAsync(dto);
            html.Setup(m => m.IsRequestPaidAsync(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestPaid);
            html.Setup(m => m.IsRequestDownloadedAndPaidAsync(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestDownloadedAndPaid);
            html.Setup(m => m.GetDownloadAsync(It.IsAny<PaymentSessionDto>())).ReturnsAsync(down);

            var indx = await controller.FetchDownloadAsync(new());
            Assert.NotNull(indx);
            var canDownload = hasUser && hasUserId && !isAccountLocked && isSessionValid && !isSessionEmpty && isRequestPaid && !isRequestDownloadedAndPaid;
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(indx);
            if (!hasUserId) Assert.IsAssignableFrom<UnauthorizedResult>(indx);
            if (isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(indx);
            if (!isRequestPaid && indx is ObjectResult notPaid)
            {
                Assert.Equal(400, notPaid.StatusCode.GetValueOrDefault());
            }
            if (!isSessionValid && indx is ObjectResult notValid)
            {
                Assert.Equal(400, notValid.StatusCode.GetValueOrDefault());
            }
            if (canDownload)
            {
                Assert.IsAssignableFrom<OkObjectResult>(indx);
            }
        }


        [Theory]
        [InlineData(true, true, false, true, false, true, true, true)]
        [InlineData(true, true, false, true, false, true, false, true)]
        [InlineData(false, true, false, true, false, true, true, true)]
        [InlineData(true, false, false, true, false, true, true, true)]
        [InlineData(true, true, true, true, false, true, true, true)]
        [InlineData(true, true, false, true, false, false, true, true)]
        [InlineData(true, true, false, false, false, true, true, true)]
        [InlineData(true, true, false, true, true, true, true, true)]
        [InlineData(true, true, false, true, false, true, true, false)]
        public async Task ControllerCanRollbackDownloadAsync(
            bool hasUser,
            bool hasUserId,
            bool isAccountLocked,
            bool isSessionValid,
            bool isSessionEmpty,
            bool isRequestPaid,
            bool isRequestDownloadedAndPaid,
            bool isResetSuccess)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secretSvc = new Mock<IClientSecretService>();
            var userId = hasUserId ? "1234567" : string.Empty;
            User? user = hasUser ? new User { Id = userId, UserName = new Faker().Person.UserName } : null;
            DownloadResponse? down = isResetSuccess ? new DownloadResponse() : null;
            var sessionJs = isSessionEmpty ? string.Empty : "abcd";
            PaymentSessionDto? dto = isSessionValid ? new() { Id = "1223456", JsText = sessionJs } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object, secretSvc.Object);

            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            lockdb.Setup(m => m.IsAccountLockedAsync(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            html.Setup(m => m.IsSessionValidAsync(It.IsAny<string>())).ReturnsAsync(dto);
            html.Setup(m => m.IsRequestPaidAsync(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestPaid);
            html.Setup(m => m.IsRequestDownloadedAndPaidAsync(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestDownloadedAndPaid);
            html.Setup(m => m.ResetDownloadAsync(It.IsAny<DownloadResetRequest>())).ReturnsAsync(down);

            var indx = await controller.RollbackDownloadAsync(new() { UserId = user?.UserName ?? "other", ExternalId = "abc-123" });
            Assert.NotNull(indx);
        }



        private static string GetPaymentLandingContent(bool isValid)
        {
            var content =
                isValid ? ApiResources.page_payment_completed
                : ApiResources.page_payment_detail_invalid;
            return content;
        }

        private static string GetUserLevelLandingContent(bool isValid)
        {
            var content =
                isValid ? ApiResources.page_payment_completed
                : ApiResources.page_level_request_completed;
            return content;

        }
    }
}
