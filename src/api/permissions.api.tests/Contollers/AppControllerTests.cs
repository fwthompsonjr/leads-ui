using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Http;
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
            var error = await Record.ExceptionAsync(async () =>
            {
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


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(11)]
        public async Task ControllerCanCreateAccountAsync(int conditionId)
        {
            var exclusions = new int[] { 0, 11 };
            var request = LeadUserBoGenerator.GetAccount();
            var loginrsp = Guid.NewGuid().ToString();
            if (conditionId == 1) { request.UserName = "admin"; } // min length error
            if (conditionId == 2) { request.UserName = fkr.Random.AlphaNumeric(55); } // max length error
            if (conditionId == 3) { request.UserName = string.Empty; } // required error

            if (conditionId == 4) { request.Password = "weak"; } // min length error
            if (conditionId == 5) { request.Password = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 6) { request.Password = string.Empty; } // required error
            if (conditionId == 7) { request.Password = "abcdefghijklmnop"; } // password strength error

            if (conditionId == 8) { request.Email = "admin"; } // min length error
            if (conditionId == 9) { request.Email = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 11) { loginrsp = string.Empty; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);
                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);
                mock.Setup(m => m.CreateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);
                var response = await sut.CreateAccountAsync(request);
                if (conditionId == 11) Assert.IsAssignableFrom<ConflictResult>(response);
                if (!exclusions.Contains(conditionId)) Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public async Task ControllerCanChangePasswordAsync(int conditionId)
        {
            var exclusions = new int[] { 0, 12 };
            var request = changeFaker.Generate();
            LeadUserModel? loginrsp = GetModel();
            var changed = true;
            if (conditionId == 1) { request.UserName = "admin"; } // min length error
            if (conditionId == 2) { request.UserName = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 3) { request.UserName = string.Empty; } // required error

            if (conditionId == 8) { request.NewPassword = "weak"; } // min length error
            if (conditionId == 9) { request.NewPassword = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 10) { request.NewPassword = string.Empty; } // required error
            if (conditionId == 11) { request.NewPassword = "abcdefghijklmnop"; } // password strength error

            if (conditionId == 12) { changed = false; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);

                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);


                mock.Setup(m => m.GetModelByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);

                mock.Setup(m => m.GetUserModel(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                    .Returns(loginrsp);

                mock.Setup(m => m.ChangePasswordAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                    .ReturnsAsync(changed);

                var response = await sut.ChangePasswordAsync(request);
                if (conditionId == 12) Assert.IsAssignableFrom<ConflictResult>(response);
                if (!exclusions.Contains(conditionId)) Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        public async Task ControllerCanSetCountyCredentialAsync(int conditionId)
        {
            var exclusions = new int[] { 0, 10 };
            var request = LeadUserBoGenerator.GetCountyLoginRequest();
            LeadUserModel? loginrsp = GetModel();
            var changed = true;
            if (conditionId == 2) { request.UserName = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 3) { request.UserName = string.Empty; } // required error

            if (conditionId == 4) { request.CountyName = "admin"; } // min length error
            if (conditionId == 5) { request.CountyName = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 6) { request.CountyName = string.Empty; } // required error

            if (conditionId == 8) { request.Password = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 9) { request.Password = string.Empty; } // required error


            if (conditionId == 10) { loginrsp = null; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);

                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);

                mock.Setup(m => m.GetUserModel(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                    .Returns(loginrsp);

                mock.Setup(m => m.GetModelByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);

                mock.Setup(m => m.ChangeCountyCredentialAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                    .ReturnsAsync(changed);

                var response = await sut.SetCountyCredentialAsync(request);
                if (conditionId == 0) Assert.IsAssignableFrom<OkObjectResult>(response);
                if (conditionId == 10) Assert.IsAssignableFrom<UnauthorizedResult>(response);
                if (!exclusions.Contains(conditionId)) Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        public async Task ControllerCanSetCountyPermisionAsync(int conditionId)
        {
            var exclusions = new int[] { 0, 10 };
            var request = LeadUserBoGenerator.GetCountyPermissionRequest();
            LeadUserModel? loginrsp = GetModel();
            var changed = true;
            if (conditionId == 2) { request.UserName = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 3) { request.UserName = string.Empty; } // required error

            if (conditionId == 5) { request.CountyList = fkr.Random.AlphaNumeric(600); } // max length error
            if (conditionId == 6) { request.CountyList = string.Empty; } // required error


            if (conditionId == 10) { loginrsp = null; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);
                var okresponse = new KeyValuePair<bool, string>(true, "unit test");
                var failedresponse = new KeyValuePair<bool, string>(false, "unit test");
                var verifcation = conditionId == 7 ? failedresponse : okresponse;
                mock.Setup(m => m.VerifyCountyList(It.IsAny<string>()))
                    .Returns(verifcation);

                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);

                mock.Setup(m => m.GetUserModel(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                    .Returns(loginrsp);



                mock.Setup(m => m.GetModelByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);

                mock.Setup(m => m.ChangeCountyPermissionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                    .ReturnsAsync(changed);

                var response = await sut.SetCountyPermisionAsync(request);
                if (conditionId == 0) Assert.IsAssignableFrom<OkObjectResult>(response);
                if (conditionId == 10) Assert.IsAssignableFrom<UnauthorizedResult>(response);
                if (!exclusions.Contains(conditionId)) Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(10)]
        public async Task ControllerCanAddCountyUsageAsync(int conditionId)
        {
            var exclusions = new int[] { 0, 10 };
            var request = LeadUserBoGenerator.GetCountyUsageRequest();
            LeadUserModel? loginrsp = GetModel();
            var changed = true;
            if (conditionId == 2) { request.UserName = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 3) { request.UserName = string.Empty; } // required error

            if (conditionId == 5) { request.CountyName = fkr.Random.AlphaNumeric(600); } // max length error
            if (conditionId == 6) { request.CountyName = string.Empty; } // required error


            if (conditionId == 10) { loginrsp = null; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);
                var okresponse = new KeyValuePair<bool, string>(true, "unit test");
                var failedresponse = new KeyValuePair<bool, string>(false, "unit test");
                var verifcation = conditionId == 7 ? failedresponse : okresponse;
                mock.Setup(m => m.VerifyCountyList(It.IsAny<string>()))
                    .Returns(verifcation);

                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);

                mock.Setup(m => m.GetUserModel(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                    .Returns(loginrsp);



                mock.Setup(m => m.GetModelByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);

                mock.Setup(m => m.AddCountyUsageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                    .ReturnsAsync(changed);

                var response = await sut.SetCountyUsageAsync(request);
                if (conditionId == 0) Assert.IsAssignableFrom<OkObjectResult>(response);
                if (conditionId == 10) Assert.IsAssignableFrom<UnauthorizedResult>(response);
                if (!exclusions.Contains(conditionId)) Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            });
            Assert.Null(error);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(10)]
        public async Task ControllerCanAddCountyUsageRecordAsync(int conditionId)
        {
            var exclusions = new int[] { 0, 10 };
            var request = LeadUserBoGenerator.GetCountyUsageRequest();
            LeadUserModel? loginrsp = GetModel();
            var changed = true;
            if (conditionId == 2) { request.UserName = fkr.Random.AlphaNumeric(300); } // max length error
            if (conditionId == 3) { request.UserName = string.Empty; } // required error

            if (conditionId == 5) { request.CountyName = fkr.Random.AlphaNumeric(600); } // max length error
            if (conditionId == 6) { request.CountyName = string.Empty; } // required error


            if (conditionId == 10) { loginrsp = null; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);
                var okresponse = new KeyValuePair<bool, string>(true, "unit test");
                var failedresponse = new KeyValuePair<bool, string>(false, "unit test");
                var verifcation = conditionId == 7 ? failedresponse : okresponse;
                mock.Setup(m => m.VerifyCountyList(It.IsAny<string>()))
                    .Returns(verifcation);

                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);

                mock.Setup(m => m.GetUserModel(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                    .Returns(loginrsp);



                mock.Setup(m => m.GetModelByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);

                mock.Setup(m => m.AddCountyUsageIncidentAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
                    .ReturnsAsync(changed);

                var response = await sut.AddCountyUsageRecordAsync(request);
                if (conditionId == 0) Assert.IsAssignableFrom<OkObjectResult>(response);
                if (conditionId == 10) Assert.IsAssignableFrom<UnauthorizedResult>(response);
                if (!exclusions.Contains(conditionId)) Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public async Task ControllerCanGetUserUsageAsync(int conditionId)
        {
            var request = new UserCountyUsageRequest { CreateDate = fkr.Date.Recent() };
            LeadUserModel? loginrsp = GetModel();
            var changed = new List<GetUsageUserByIdResponse>();
            if (conditionId == 10) { loginrsp = null; }
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<AppController>();
                var mock = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
                var json = GetLoginResponse(true);

                mock.Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(json);

                mock.Setup(m => m.GetUserModel(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                    .Returns(loginrsp);



                mock.Setup(m => m.GetModelByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(loginrsp);

                mock.Setup(m => m.GetUsageUserByIdAsync(
                    It.IsAny<string>()))
                    .ReturnsAsync(changed);

                var response = await sut.GetUserUsageAsync(request);
                if (conditionId == 0) Assert.IsAssignableFrom<OkObjectResult>(response);
                if (conditionId == 10) Assert.IsAssignableFrom<UnauthorizedResult>(response);

            });
            Assert.Null(error);
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
        private static readonly Faker fkr = new();
    }
}