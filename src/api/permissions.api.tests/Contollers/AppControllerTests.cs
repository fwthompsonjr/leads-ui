using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
        [InlineData("administrator")]
        [InlineData("lead.administrator")]
        public async Task ControllerCanAccountAuthenicateAsync(string name)
        {
            const string notJson = "not serializable";
            var error = await Record.ExceptionAsync(async () => {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var isAuthenicated = name.Contains("administrator");
                var json = name switch
                {
                    "dallas" => string.Empty,
                    "denton" => notJson,
                    _ => GetLoginResponse(isAuthenicated)
                };
                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);

                var response = await sut.AccountAuthenicateAsync(new AppAuthenicateRequest { UserName = name, Password = "default" });
                if (string.IsNullOrEmpty(json)) Assert.IsAssignableFrom<UnauthorizedResult>(response);
                if (json.Equals(notJson)) Assert.IsAssignableFrom<ConflictObjectResult>(response);
                if (isAuthenicated) Assert.IsAssignableFrom<OkObjectResult>(response);

            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("dallas")]
        [InlineData("denton")]
        [InlineData("lead.administrator")]
        public void ControllerCanMockAuthenicate(string name)
        {
            var auth = new Mock<IAppAuthenicationService>();
            var county = new Mock<ICountyAuthorizationService>();
            var svcs = new Mock<ILeadAuthenicationService>().Object;
            var sut = new AppController(auth.Object, county.Object, svcs);
            var isok = name.Equals("lead.administrator");
            var dto = isok ? new AppAuthenicationItemDto { Id = 10, UserName = "test.account" } : null;
            auth.Setup(s => s.Authenicate(It.IsAny<string>(), It.IsAny<string>())).Returns(dto);
            auth.Setup(s => s.Authenicate(It.IsAny<string>(), It.IsAny<string>())).Returns(dto);
            auth.Setup(s => s.FindUser(It.IsAny<string>(), It.IsAny<int>())).Returns(dto);
            var response = sut.Authenicate(new AppAuthenicateRequest { UserName = name, Password = "default" });
            if (isok) Assert.IsType<OkObjectResult>(response);
            else Assert.IsType<UnauthorizedResult>(response);
        }

        private static string GetLoginResponse(bool authorized)
        {
            if (!authorized) return string.Empty;
            var model = GetModel();
            return JsonConvert.SerializeObject(model);
        }
        private static LeadUserModel GetModel()
        {
            var bo = LeadUserBoGenerator.GetBo(1, 1);
            return securityService.GetModel(bo);
        }

        private static readonly LeadSecurityService securityService = new();

    }
}