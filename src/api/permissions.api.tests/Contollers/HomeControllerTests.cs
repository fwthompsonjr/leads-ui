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
            var controller = new HomeController(html.Object, infrastructure.Object, subscription.Object);
            var indx = controller.Index();
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
    }
}
