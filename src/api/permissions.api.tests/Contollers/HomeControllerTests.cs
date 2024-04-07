using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object);
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
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object, lockdb.Object);
            html.Setup(m => m.IsRequestValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(isValid);
            var indx = await controller.PaymentLanding("abcd", "123");
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
    }
}
