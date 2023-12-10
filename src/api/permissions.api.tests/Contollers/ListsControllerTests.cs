using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.json.db.entity;
using legallead.json.db.interfaces;
using legallead.permissions.api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Security.Principal;

namespace permissions.api.tests.Contollers
{
    public class ListsControllerTests : BaseControllerTest
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<ListsController>();
            Assert.NotNull(sut);
        }

        [Fact]
        public void ControllerCanGetStateDetails()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<ListsController>();
            var actual = sut.GetStateDetails();
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }

        [Fact]
        public void ControllerCanGetCountyDetails()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<ListsController>();
            var actual = sut.GetCountyDetails();
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }

        [Fact]
        public async Task GetPermissionGroupsHappyPath()
        {
            var fk = new Faker();
            var provider = GetProvider();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
            var sut = provider.GetRequiredService<ListsController>();

            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;
            sut.ControllerContext.HttpContext.User = principal;
            userDb.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(new User());
            mockRq.SetupGet(m => m.HttpContext).Returns(sut.HttpContext);

            var actual = await sut.GetPermissionGroups();
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }

        [Fact]
        public async Task GetUserProfileHappyPath()
        {
            var fk = new Faker();
            var provider = GetProvider();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
            var sut = provider.GetRequiredService<ListsController>();

            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;
            sut.ControllerContext.HttpContext.User = principal;
            userDb.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(new User());
            mockRq.SetupGet(m => m.HttpContext).Returns(sut.HttpContext);

            var actual = await sut.GetUserProfile();
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }

        [Fact]
        public async Task GetUserPermissionsHappyPath()
        {
            var fk = new Faker();
            var provider = GetProvider();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDb = provider.GetRequiredService<Mock<IUserRepository>>();
            var sut = provider.GetRequiredService<ListsController>();

            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;
            sut.ControllerContext.HttpContext.User = principal;
            userDb.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(new User());
            mockRq.SetupGet(m => m.HttpContext).Returns(sut.HttpContext);

            var actual = await sut.GetUserPermissions();
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }
    }
}