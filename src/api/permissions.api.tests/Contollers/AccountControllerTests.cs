using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Principal;

namespace permissions.api.tests.Contollers
{
    public class AccountControllerTests : BaseControllerTest
    {
        private static readonly Faker<UserLoginModel> faker =
            new Faker<UserLoginModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(22));

        private static readonly Faker<Tokens> tokenFaker =
            new Faker<Tokens>()
            .RuleFor(x => x.RefreshToken, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.AccessToken, y => y.Random.AlphaNumeric(22));

        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<AccountController>();
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task AuthenicateRequiresAppHeader()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<AccountController>();
            var request = faker.Generate();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            mockRq.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("An error occurred authenticating account.", objectResponse);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsValidUserResponse()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(false, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<AccountController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid username or password...", objectResponse);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsNotNullUserResponse()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<AccountController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid username or password...", objectResponse);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsNotEmptyUserId()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User());
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<AccountController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid username or password...", objectResponse);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsNotNullTokenResponse()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            Tokens? tokens = null;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GenerateToken(It.IsAny<User>())).Returns(tokens);

            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid Attempt..", objectResponse);
            }
        }

        [Fact]
        public async Task AuthenicateHappyPath()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            Tokens? tokens = new() { RefreshToken = "123-456-789" };

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GenerateToken(It.IsAny<User>())).Returns(tokens);

            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);
            if (actual is OkObjectResult objectResult && objectResult.Value is Tokens objectResponse)
            {
                Assert.Equal(tokens.RefreshToken, objectResponse.RefreshToken);
            }
            else
            {
                Assert.Fail("Invalid Controller Response.");
            }
        }

        [Fact]
        public async Task RefreshRequiresAppHeader()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<AccountController>();
            var request = tokenFaker.Generate();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            mockRq.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            var actual = await sut.Refresh(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("An error occurred refreshing authentication token.", objectResponse);
            }
        }


        [Fact]
        public async Task RefreshRequiresNonNullClaimsPrincipal()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            ClaimsPrincipal? principal = null;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.Refresh(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullClaimsIdentity()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            claimMq.SetupGet(m => m.Identity).Returns((IIdentity?)null);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.Refresh(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullClaimsIdentityName()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            identityMq.SetupGet(m => m.Name).Returns(string.Empty);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.Refresh(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
            else
            {
                Assert.Fail("Invalid Controller Response.");
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullUser()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            identityMq.SetupGet(m => m.Name).Returns(new Faker().Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.Refresh(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("User data is null or empty.", objectResponse);
            }
            else
            {
                Assert.Fail("Invalid Controller Response.");
            }
        }
        [Fact]
        public async Task RefreshRequiresNonEmptyUserId()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = string.Empty });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<AccountController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            identityMq.SetupGet(m => m.Name).Returns(new Faker().Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.Refresh(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("User data is null or empty.", objectResponse);
            }
            else
            {
                Assert.Fail("Invalid Controller Response.");
            }
        }
    }
}
