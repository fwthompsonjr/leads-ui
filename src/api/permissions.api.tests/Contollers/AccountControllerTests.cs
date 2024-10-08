﻿using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Security.Principal;

namespace permissions.api.tests.Contollers
{
    public class AccountControllerTests : BaseControllerTest
    {
        private const string unexpectedReply = "Unexpected Controller Response.";

        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<SignonController>();
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task AuthenicateRequiresAppHeaderAsync()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<SignonController>();
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
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsValidUserResponseAsync()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(false, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

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
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsNotNullUserResponseAsync()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

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
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsNotEmptyUserIdAsync()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User());
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

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
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task AuthenicateExpectsNotNullTokenResponseAsync()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
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
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task AuthenicateHappyPathAsync()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
            Tokens? tokens = new() { RefreshToken = "123-456-789", Expires = DateTime.UtcNow.AddMinutes(1) };

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
                Assert.True(tokens.Expires.HasValue);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task AuthenicateCatchesExceptionsAsync()
        {
            var provider = GetProvider();
            var request = faker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GenerateToken(It.IsAny<User>())).Throws(new ApplicationException());

            var actual = await sut.AuthenticateAsync(request);
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task RefreshRequiresAppHeaderAsync()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<SignonController>();
            var request = tokenFaker.Generate();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            mockRq.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("An error occurred refreshing authentication token.", objectResponse);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullClaimsPrincipalAsync()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
            ClaimsPrincipal? principal = null;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullClaimsIdentityAsync()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            claimMq.SetupGet(m => m.Identity).Returns((IIdentity?)null);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullClaimsIdentityNameAsync()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "abcd" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            identityMq.SetupGet(m => m.Name).Returns(string.Empty);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonNullUserAsync()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            identityMq.SetupGet(m => m.Name).Returns(new Faker().Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("User data is null or empty.", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task RefreshRequiresNonEmptyUserIdAsync()
        {
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = string.Empty });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            identityMq.SetupGet(m => m.Name).Returns(new Faker().Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("User data is null or empty.", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task RefreshRequiresNotNullRefreshTokenAsync()
        {
            var fk = new Faker();
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = fk.Random.Guid().ToString() });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var userTokenMq = provider.GetRequiredService<Mock<IUserTokenRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var validMq = provider.GetRequiredService<Mock<IRefreshTokenValidator>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            UserRefreshToken? findResponse = null;
            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            userTokenMq.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(findResponse);
            validMq.Setup(m => m.Verify(It.IsAny<UserRefreshToken?>())).Returns(findResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task RefreshRequiresActiveRefreshTokenAsync()
        {
            var fk = new Faker();
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = fk.Random.Guid().ToString() });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var userTokenMq = provider.GetRequiredService<Mock<IUserTokenRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var validMq = provider.GetRequiredService<Mock<IRefreshTokenValidator>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            var findResponse = new UserRefreshToken
            {
                UserId = fk.Random.Guid().ToString(),
                RefreshToken = fk.Random.AlphaNumeric(30),
                IsActive = false
            };
            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            userTokenMq.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(findResponse);
            validMq.Setup(m => m.Verify(It.IsAny<UserRefreshToken?>())).Returns(findResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task RefreshRequiresNonNullJwtResponseAsync()
        {
            var fk = new Faker();
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = fk.Random.Guid().ToString() });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var userTokenMq = provider.GetRequiredService<Mock<IUserTokenRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var validMq = provider.GetRequiredService<Mock<IRefreshTokenValidator>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            var findResponse = new UserRefreshToken
            {
                UserId = fk.Random.Guid().ToString(),
                RefreshToken = fk.Random.AlphaNumeric(30),
                IsActive = true
            };
            Tokens? jwtResponse = null;
            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            userTokenMq.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(findResponse);
            validMq.Setup(m => m.Verify(It.IsAny<UserRefreshToken?>())).Returns(findResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            jwtMq.Setup(m => m.GenerateRefreshToken(It.IsAny<User>())).Returns(jwtResponse);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task RefreshHappyPathAsync()
        {
            var fk = new Faker();
            var request = tokenFaker.Generate();
            var provider = GetProvider();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = fk.Random.Guid().ToString() });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var userTokenMq = provider.GetRequiredService<Mock<IUserTokenRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var validMq = provider.GetRequiredService<Mock<IRefreshTokenValidator>>();
            var sut = provider.GetRequiredService<SignonController>();
            var claimMq = new Mock<ClaimsPrincipal>();
            var identityMq = new Mock<IIdentity>();
            var findResponse = new UserRefreshToken
            {
                UserId = fk.Random.Guid().ToString(),
                RefreshToken = fk.Random.AlphaNumeric(30),
                IsActive = true
            };
            var jwtResponse = new Tokens
            {
                RefreshToken = findResponse.RefreshToken,
                AccessToken = fk.Music.Genre()
            };
            identityMq.SetupGet(m => m.Name).Returns(fk.Person.Email);
            claimMq.SetupGet(m => m.Identity).Returns(identityMq.Object);
            ClaimsPrincipal? principal = claimMq.Object;

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.GetByEmail(It.IsAny<string>())).ReturnsAsync(userResponse.Value);
            userTokenMq.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(findResponse);
            validMq.Setup(m => m.Verify(It.IsAny<UserRefreshToken?>())).Returns(findResponse);
            jwtMq.Setup(m => m.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(principal);
            jwtMq.Setup(m => m.GenerateRefreshToken(It.IsAny<User>())).Returns(jwtResponse);
            var actual = await sut.RefreshAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);
            if (actual is OkObjectResult objectResult && objectResult.Value is Tokens objectResponse)
            {
                Assert.Equal(jwtResponse.RefreshToken, objectResponse.RefreshToken);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public void VerifyRequiresAppHeader()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<SignonController>();
            var request = tokenFaker.Generate();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            mockRq.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            var actual = sut.Verify(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("An error occurred verifying authentication token.", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public void VerifyRequiresValidToken()
        {
            var provider = GetProvider();
            var request = tokenFaker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "1234" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.ValidateToken(It.IsAny<string>())).Returns(false);

            var actual = sut.Verify(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            if (actual is BadRequestObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid access token.", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public void VerifyHappyPath()
        {
            var provider = GetProvider();
            var request = tokenFaker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User { Id = "1234" });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var jwtMq = provider.GetRequiredService<Mock<IJwtManagerRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);
            jwtMq.Setup(m => m.ValidateToken(It.IsAny<string>())).Returns(true);

            var actual = sut.Verify(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);
            if (actual is OkObjectResult objectResult && objectResult.Value is bool objectResponse)
            {
                Assert.True(objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task ChangePasswordRequiresAppHeaderAsync()
        {
            var provider = GetProvider();
            var sut = provider.GetRequiredService<SignonController>();
            var request = changeFaker.Generate();
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            mockRq.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            var actual = await sut.ChangePasswordAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("An error occurred authenticating account.", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task ChangePasswordExpectsValidUserResponseAsync()
        {
            var provider = GetProvider();
            var request = changeFaker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(false, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.ChangePasswordAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid username or password...", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task ChangePasswordExpectsNotNullUserResponseAsync()
        {
            var provider = GetProvider();
            var request = changeFaker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, null);
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.ChangePasswordAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid username or password...", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task ChangePasswordExpectsNotEmptyUserIdAsync()
        {
            var provider = GetProvider();
            var request = changeFaker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true, new User());
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.ChangePasswordAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(actual);
            if (actual is UnauthorizedObjectResult objectResult && objectResult.Value is string objectResponse)
            {
                Assert.Equal("Invalid username or password...", objectResponse);
            }
            else
            {
                Assert.Fail(unexpectedReply);
            }
        }

        [Fact]
        public async Task ChangePasswordHappyPathAsync()
        {
            var fkr = new Faker();
            var provider = GetProvider();
            var request = changeFaker.Generate();
            var appHeader = GetApplicationHeader();
            var userResponse = new KeyValuePair<bool, User?>(true,
                new User
                {
                    Id = fkr.Random.Guid().ToString(),
                    UserName = fkr.Random.AlphaNumeric(16),
                    Email = fkr.Person.Email,
                    PasswordHash = fkr.Random.AlphaNumeric(24),
                    PasswordSalt = fkr.Random.AlphaNumeric(24)
                });
            var mockRq = provider.GetRequiredService<Mock<HttpRequest>>();
            var userDbMq = provider.GetRequiredService<Mock<IUserRepository>>();
            var compDbMq = provider.GetRequiredService<Mock<IComponentRepository>>();
            var sut = provider.GetRequiredService<SignonController>();

            mockRq.SetupGet(x => x.Headers).Returns(appHeader.Headers);
            compDbMq.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(appHeader.App);
            userDbMq.Setup(m => m.IsValidUserAsync(It.IsAny<UserModel>())).ReturnsAsync(userResponse);

            var actual = await sut.ChangePasswordAsync(request);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);
            if (!(actual is OkObjectResult objectResult && objectResult.Value is string))
            {
                Assert.Fail(unexpectedReply);
            }
        }
    }
}