using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace permissions.api.tests.Contollers
{
    public class AppControllerTests : BaseControllerTest
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<AppController>();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData("dallas", false)]
        [InlineData("denton", true)]
        public void ControllerCanGetCounty(string name, bool expected)
        {

            var sut = GetProvider().GetRequiredService<AppController>();
            var response = sut.GetCounty(new() { Name = name, UserId = "default" });
            if (response is not OkObjectResult result)
            {
                Assert.Fail("Controller response not matched to expected.");
                return;
            }
            if (result.Value is not AuthorizedCountyModel model)
            {
                Assert.Fail("Controller response not matched to expected type.");
                return;
            }
            Assert.Equal(expected, string.IsNullOrEmpty(model.Name));
            Assert.Equal(expected, string.IsNullOrEmpty(model.Code));
        }


        [Theory]
        [InlineData("dallas")]
        [InlineData("denton")]
        [InlineData("lead.administrator")]
        public void ControllerCanAuthenicate(string name)
        {

            var sut = GetProvider().GetRequiredService<AppController>();
            var response = sut.Authenicate(new AppAuthenicateRequest { UserName = name, Password = "default" });
            if (response is not UnauthorizedResult _)
            {
                Assert.Fail("Controller response not matched to expected.");
                return;
            }
        }
        [Theory]
        [InlineData("dallas")]
        [InlineData("denton")]
        [InlineData("lead.administrator")]
        public void ControllerCanMockAuthenicate(string name)
        {
            var auth = new Mock<IAppAuthenicationService>();
            var county = new Mock<ICountyAuthorizationService>();
            var sut = new AppController(auth.Object, county.Object);
            var isok = name.Equals("lead.administrator");
            var dto = isok ? new AppAuthenicationItemDto { Id = 10, UserName = "test.account" } : null;
            auth.Setup(s => s.Authenicate(It.IsAny<string>(), It.IsAny<string>())).Returns(dto);
            auth.Setup(s => s.Authenicate(It.IsAny<string>(), It.IsAny<string>())).Returns(dto);
            auth.Setup(s => s.FindUser(It.IsAny<string>(), It.IsAny<int>())).Returns(dto);
            var response = sut.Authenicate(new AppAuthenicateRequest { UserName = name, Password = "default" });
            if (isok) Assert.IsType<OkObjectResult>(response);
            else Assert.IsType<UnauthorizedResult>(response);
        }

    }
}