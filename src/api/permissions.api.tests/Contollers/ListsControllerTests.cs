using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
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
        private static readonly object locker = new();

        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<ListsController>();
            Assert.NotNull(sut);
        }

        [Fact]
        public void ControllerCanGetStateDetails()
        {
            lock (locker)
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<ListsController>();
                var actual = sut.GetStateDetails();
                Assert.NotNull(actual);
                Assert.IsType<OkObjectResult>(actual);
            }
        }

        [Fact]
        public void ControllerCanGetCountyDetails()
        {
            lock (locker)
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<ListsController>();
                var actual = sut.GetCountyDetails();
                Assert.NotNull(actual);
                Assert.IsType<OkObjectResult>(actual);
            }
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2925:\"Thread.Sleep\" should not be used in tests",
            Justification = "Code refactor pending to avoid flaky test.")]
        public async Task GetPermissionGroupsHappyPathAsync()
        {
            var retries = 3;
            while (retries > 0)
            {
                var exception = await Record.ExceptionAsync(async () =>
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

                    var actual = await sut.GetPermissionGroupsAsync();
                    Assert.NotNull(actual);
                    Assert.IsType<OkObjectResult>(actual);
                });
                if (exception == null) break;
                retries--;
                if (retries == 0) Assert.Null(exception);
                Thread.Sleep(200);
            }
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2925:\"Thread.Sleep\" should not be used in tests",
            Justification = "Code refactor pending to avoid flaky test.")]
        public async Task GetUserProfileHappyPathAsync()
        {
            var retries = 3;
            while (retries > 0)
            {
                var exception = await Record.ExceptionAsync(async () =>
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

                    var actual = await sut.GetUserProfileAsync();
                    Assert.NotNull(actual);
                    Assert.IsType<OkObjectResult>(actual);
                });
                if (exception == null) break;
                retries--;
                if (retries == 0) Assert.Null(exception);
                Thread.Sleep(200);
            }
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2925:\"Thread.Sleep\" should not be used in tests",
            Justification = "Code refactor pending to avoid flaky test.")]
        public async Task GetUserPermissionsHappyPathAsync()
        {
            var retries = 3;
            while (retries > 0)
            {
                var exception = await Record.ExceptionAsync(async () =>
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

                    var actual = await sut.GetUserPermissionsAsync();
                    Assert.NotNull(actual);
                    Assert.IsType<OkObjectResult>(actual);
                });
                if (exception == null) break;
                retries--;
                if (retries == 0) Assert.Null(exception);
                Thread.Sleep(200);
            }
        }
    }
}