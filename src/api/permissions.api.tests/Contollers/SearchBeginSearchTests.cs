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
    public class SearchBeginSearchTests : BaseSearchControllerTest
    {
        [Fact]
        public void ControllerCanBeConstructed()
        {
            var provider = GetServiceProvider();
            var controller = provider.GetRequiredService<SearchController>();
            Assert.NotNull(controller);
        }

        [Theory]
        [InlineData(true, false, true, true, true)]
        [InlineData(false, false, true, true, true)]
        [InlineData(true, true, true, true, true)]
        public async Task ControllerCanBeginSearch(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse,
            bool hasResponseIndex)
        {
            var request = fakerSearchRequest.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            UserSearchBeginResponse? response = hasResponse ? fakerSearchBegin.Generate() : null;
            if (response != null && !hasResponseIndex) response.RequestId = string.Empty;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.Begin(It.IsAny<HttpRequest>(), It.IsAny<UserSearchRequest>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.BeginSearch(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }


        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        public async Task ControllerCanPostMySearches(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            UserSearchQueryModel[]? response = hasResponse ? Array.Empty<UserSearchQueryModel>() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetHeader(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.MySearches(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        public async Task ControllerCanPostMySearchesCount(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            UserSearchQueryModel[]? response = hasResponse ? Array.Empty<UserSearchQueryModel>() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetHeader(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.MySearchesCount(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        public async Task ControllerCanGetMyActiveSearches(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            UserSearchQueryModel[]? response = hasResponse ? Array.Empty<UserSearchQueryModel>() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetSearchDetails(It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.MyActiveSearches(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(true, false, true, false)]
        public async Task ControllerCanGetPreview(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            SearchPreviewBo[]? response = hasResponse ? Array.Empty<SearchPreviewBo>() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetPreview(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.Preview(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(true, false, true, false)]
        public async Task ControllerCanGetSearchStatus(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            ActiveSearchOverviewBo? response = hasResponse ? new() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetSearchProgress(It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.SearchStatus(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(true, false, true, false)]
        public async Task ControllerCanGetRestrictionStatus(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            SearchRestrictionModel? response = hasResponse ? new() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetRestrictionStatus(It.IsAny<HttpRequest>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.RestrictionStatus(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
        }
        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        public async Task ControllerCanGetMyPurchases(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse)
        {
            var request = modelfaker.Generate();
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            PurchasedSearchBo[]? response = hasResponse ? Array.Empty<PurchasedSearchBo>() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetPurchases(It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.MyPurchases(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        [Theory]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, true, true, true)]
        public async Task ControllerCanListMyPurchases(
            bool hasUser,
            bool isAccountLocked,
            bool isValid,
            bool hasResponse,
            bool isEmptyResponse = false)
        {
            var provider = GetServiceProvider();
            var valid = provider.GetRequiredService<Mock<IUserSearchValidator>>();
            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var validation = new KeyValuePair<bool, string>(isValid, "unit testing");
            User? user = hasUser ? userfaker.Generate() : null;
            List<PurchasedSearchBo>? response = hasResponse ? purchaseFaker.Generate(10) : null;
            if (isEmptyResponse) response?.Clear();
            var request = user?.UserName ?? "unset";
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetPurchases(It.IsAny<string>())).ReturnsAsync(response);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isAccountLocked);
            valid.Setup(s => s.IsValid(It.IsAny<UserSearchRequest>())).Returns(validation);
            var controller = provider.GetRequiredService<SearchController>();
            var result = await controller.ListMyPurchases(request);
            Assert.NotNull(result);
            if (!hasUser) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && isAccountLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
    }
}
