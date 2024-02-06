using legallead.permissions.api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace permissions.api.tests.Contollers
{
    public class HomeControllerTests
    {
        [Fact]
        public void ControllerCanGetIndex()
        {
            var controller = new HomeController();
            var indx = controller.Index();
            Assert.NotNull(indx);
            Assert.IsType<ContentResult>(indx);
        }
    }
}
