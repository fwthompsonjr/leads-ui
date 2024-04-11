using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace permissions.api.tests.Contollers
{
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
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            var indx = controller.Index();
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetPaymentLanding(bool isValid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsRequestValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            var indx = await controller.PaymentLanding("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ControllerCanGetPaymentCheckout(bool isSessionValid, bool isRequestPaid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            PaymentSessionDto? session = isSessionValid ? new PaymentSessionDto() : null;
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsRequestValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            html.Setup(m => m.IsSessionValid(It.IsAny<string>())).ReturnsAsync(session);
            html.Setup(m => m.IsRequestPaid(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestPaid);
            var indx = await controller.PaymentCheckout("abcd");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetSubscriptionLanding(bool isValid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsChangeUserLevel(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            var indx = await controller.UserLevelLanding("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ControllerCanGetSubscriptionCheckout(bool isSessionValid, bool isRequestPaid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            LevelRequestBo? session = isSessionValid ? new LevelRequestBo() : null;
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsChangeUserLevel(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            html.Setup(m => m.IsSubscriptionValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(session);
            html.Setup(m => m.IsRequestPaid(It.IsAny<LevelRequestBo>())).ReturnsAsync(isRequestPaid);
            var indx = await controller.SubscriptionCheckout("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ControllerCanGetDiscountCheckout(bool isSessionValid, bool isRequestPaid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            LevelRequestBo? session = isSessionValid ? new LevelRequestBo() : null;
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsDiscountLevel(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            html.Setup(m => m.IsDiscountValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(session);
            html.Setup(m => m.IsDiscountPaid(It.IsAny<LevelRequestBo>())).ReturnsAsync(isRequestPaid);
            var indx = await controller.DiscountCheckout("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetDiscountLanding(bool isValid)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsDiscountLevel(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            var indx = await controller.DiscountLanding("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ControllerCanGetIntentLanding(bool isValid, bool hasClientId)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var clientId = hasClientId ? "abcd" : string.Empty;
            PaymentSessionDto? dto = isValid ? new() { ClientId = clientId } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            html.Setup(m => m.IsSessionValid(It.IsAny<string>())).ReturnsAsync(dto);
            var indx = await controller.FetchIntent(new());
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
        public async Task ControllerCanFetchSubscriptionIntent(bool isValid, bool hasSessionId)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secret = new { Id = "00000" };
            var sessionId = hasSessionId ? "abcd" : string.Empty;
            LevelRequestBo? dto = isValid ? new() { SessionId = sessionId } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            subscription.Setup(m => m.GetLevelRequestById(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dto);
            stripeSvcs.Setup(m => m.FetchClientSecret(It.IsAny<LevelRequestBo>())).ReturnsAsync(secret);
            var indx = await controller.FetchSubscriptionIntent(new());
            Assert.NotNull(indx);
            Assert.IsType<JsonResult>(indx);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ControllerCanFetchDiscountIntent(bool isValid, bool hasSessionId)
        {
            var html = new Mock<IPaymentHtmlTranslator>();
            var infrastructure = new Mock<ISearchInfrastructure>();
            var subscription = new Mock<ISubscriptionInfrastructure>();
            var lockdb = new Mock<ICustomerLockInfrastructure>();
            var stripeSvcs = new Mock<IStripeInfrastructure>();
            var secret = new { Id = "00000" };
            var sessionId = hasSessionId ? "abcd" : string.Empty;
            LevelRequestBo? dto = isValid ? new() { SessionId = sessionId } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);
            subscription.Setup(m => m.GetDiscountRequestById(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dto);
            stripeSvcs.Setup(m => m.FetchClientSecret(It.IsAny<LevelRequestBo>())).ReturnsAsync(secret);
            var indx = await controller.FetchDiscountIntent(new());
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
        public async Task ControllerCanFetchDownload(
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
            var userId = hasUserId ? "1234567" : string.Empty;
            User? user = hasUser ? new User { Id = userId } : null;
            var down = new DownloadResponse();
            var sessionJs = isSessionEmpty ? string.Empty : "abcd";
            PaymentSessionDto? dto = isSessionValid ? new() { Id = "1223456", JsText = sessionJs } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);

            infrastructure.Setup(m => m.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            lockdb.Setup(m => m.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            html.Setup(m => m.IsSessionValid(It.IsAny<string>())).ReturnsAsync(dto);
            html.Setup(m => m.IsRequestPaid(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestPaid);
            html.Setup(m => m.IsRequestDownloadedAndPaid(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestDownloadedAndPaid);
            html.Setup(m => m.GetDownload(It.IsAny<PaymentSessionDto>())).ReturnsAsync(down);

            var indx = await controller.FetchDownload(new());
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
        public async Task ControllerCanRollbackDownload(
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
            var userId = hasUserId ? "1234567" : string.Empty;
            User? user = hasUser ? new User { Id = userId, UserName = new Faker().Person.UserName } : null;
            DownloadResponse? down = isResetSuccess ? new DownloadResponse() : null;
            var sessionJs = isSessionEmpty ? string.Empty : "abcd";
            PaymentSessionDto? dto = isSessionValid ? new() { Id = "1223456", JsText = sessionJs } : null;
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object, stripeSvcs.Object);

            infrastructure.Setup(m => m.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            lockdb.Setup(m => m.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            html.Setup(m => m.IsSessionValid(It.IsAny<string>())).ReturnsAsync(dto);
            html.Setup(m => m.IsRequestPaid(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestPaid);
            html.Setup(m => m.IsRequestDownloadedAndPaid(It.IsAny<PaymentSessionDto>())).ReturnsAsync(isRequestDownloadedAndPaid);
            html.Setup(m => m.ResetDownload(It.IsAny<DownloadResetRequest>())).ReturnsAsync(down);

            var indx = await controller.RollbackDownload(new() { UserId = user?.UserName ?? "other", ExternalId = "abc-123"});
            Assert.NotNull(indx);
        }
    }
}
