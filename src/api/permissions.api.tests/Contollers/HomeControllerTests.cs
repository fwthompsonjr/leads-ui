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
            var controller = new HomeController(html.Object);
            var indx = controller.Index();
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
    }
}
