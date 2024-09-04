using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class SearchControllerTests : BaseControllerTest
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var sut = GetProvider().GetRequiredService<SearchController>();
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task ControllerCanBeginSearchHappyPathAsync()
        {
            var user = new User();
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            var response = new UserSearchBeginResponse { RequestId = "000-000-000", Request = new() };
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.BeginAsync(It.IsAny<HttpRequest>(), It.IsAny<UserSearchRequest>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.BeginSearchAsync(new UserSearchRequest());
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }

        [Fact]
        public async Task ControllerCanBeginNoUserReturnsUnauthorizedAsync()
        {
            User? user = default;
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            var response = new UserSearchBeginResponse { RequestId = "000-000-000", Request = new() };
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.BeginAsync(It.IsAny<HttpRequest>(), It.IsAny<UserSearchRequest>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.BeginSearchAsync(new UserSearchRequest());
            Assert.NotNull(actual);
            Assert.IsType<UnauthorizedResult>(actual);
        }
        [Fact]
        public async Task ControllerCanBeginSearchInvalidParameterAsync()
        {
            var user = new User();
            var vrsp = new KeyValuePair<bool, string>(false, "test response");
            var response = new UserSearchBeginResponse { RequestId = "000-000-000", Request = new() };
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.BeginAsync(It.IsAny<HttpRequest>(), It.IsAny<UserSearchRequest>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.BeginSearchAsync(new UserSearchRequest());
            Assert.NotNull(actual);
            Assert.IsType<BadRequestObjectResult>(actual);
        }

        [Fact]
        public async Task ControllerCanBeginSearchNullResultShouldBeConflictAsync()
        {
            var user = new User();
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            UserSearchBeginResponse? response = default;
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.BeginAsync(It.IsAny<HttpRequest>(), It.IsAny<UserSearchRequest>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.BeginSearchAsync(new UserSearchRequest());
            Assert.NotNull(actual);
            Assert.IsType<ConflictObjectResult>(actual);
        }

        [Fact]
        public async Task ControllerCanBeginSearchEmptyResponeIsUnprocessableEntityAsync()
        {
            var user = new User();
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            var response = new UserSearchBeginResponse { RequestId = "", Request = new() };
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.BeginAsync(It.IsAny<HttpRequest>(), It.IsAny<UserSearchRequest>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.BeginSearchAsync(new UserSearchRequest());
            Assert.NotNull(actual);
            Assert.IsType<UnprocessableEntityObjectResult>(actual);
        }

        [Fact]
        public async Task ControllerCanGetMySearchesHappyPathAsync()
        {
            var user = new User();
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            var response = new List<UserSearchQueryModel>();
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.GetHeaderAsync(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.MySearchesAsync(new ApplicationModel { Id = Guid.NewGuid().ToString() });
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
        }

        [Fact]
        public async Task ControllerCanGetMySearchesNoUserIsUnauthorizedAsync()
        {
            User? user = default;
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            var response = new List<UserSearchQueryModel>();
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.GetHeaderAsync(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.MySearchesAsync(new ApplicationModel { Id = Guid.NewGuid().ToString() });
            Assert.NotNull(actual);
            Assert.IsType<UnauthorizedResult>(actual);
        }

        [Fact]
        public async Task ControllerCanGetMySearchesNoGuidIsUnauthorizedAsync()
        {
            var user = new User();
            var vrsp = new KeyValuePair<bool, string>(true, "test response");
            var response = new List<UserSearchQueryModel>();
            var provider = GetProvider();
            var validator = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infrastructure = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var sut = provider.GetRequiredService<SearchController>();
            infrastructure.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infrastructure.Setup(m => m.GetHeaderAsync(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(response);
            validator.Setup(m => m.IsValid(It.IsAny<UserSearchRequest>())).Returns(vrsp);
            var actual = await sut.MySearchesAsync(new ApplicationModel { Id = "not-a-guid" });
            Assert.NotNull(actual);
            Assert.IsType<UnauthorizedResult>(actual);
        }
    }
}
