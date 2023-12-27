using Bogus;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.tests.utilities
{
    public class PermissionApiGetTests
    {
        private readonly Faker faker = new();

        [Fact]
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var api = new PermissionPageClient(faker.Internet.DomainName());
                Assert.NotNull(api.InternetUtility);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("read-me", true)]
        [InlineData("list", true)]
        [InlineData("login", false)]
        [InlineData("misspelled", false)]
        public void ServiceCanGetPageUrl(string landing, bool expected)
        {
            var mock = new ActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var actual = service.CanGet(landing).Key;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", false)]
        [InlineData("list", false)]
        [InlineData("login", false)]
        [InlineData("misspelled", false)]
        public void ServiceCanNotGetPageUrlWithoutInternet(string landing, bool expected)
        {
            var mock = new InActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var actual = service.CanGet(landing).Key;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", false)]
        [InlineData("list", false)]
        [InlineData("misspelled", false)]
        [InlineData("login", true)]
        [InlineData("register", true)]
        [InlineData("refresh", true)]
        [InlineData("change-password", true)]
        public void ServiceCanGetPostPageUrl(string landing, bool expected)
        {
            var mock = new ActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var user = GetUser(faker);
            var actual = service.CanPost(landing, new(), user).Key;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("read-me", false)]
        [InlineData("list", false)]
        [InlineData("misspelled", false)]
        [InlineData("login", false)]
        [InlineData("register", false)]
        public void ServiceCanNotGetPostPageWithNonInitializedUser(string landing, bool expected)
        {
            var mock = new ActiveInternetStatus();
            var service = new PermissionPageClient(faker.Internet.DomainName(), mock);
            var user = GetUser(faker, false);
            var actual = service.CanPost(landing, new(), user).Key;
            Assert.Equal(expected, actual);
        }

        private static UserBo GetUser(Faker faker, bool isInitialized = true)
        {
            var user = new UserBo()
            {
                UserName = faker.Person.Email,
                Applications = new ApiContext[] { new() { Id = Guid.Empty.ToString(), Name = "legallead.desktop.tests" } }
            };
            if (isInitialized) return user;
            user.Applications = Array.Empty<ApiContext>();
            return user;
        }

        private sealed class ActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }

        private sealed class InActiveInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return false;
            }
        }
    }
}