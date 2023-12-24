using Bogus;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Moq;

namespace legallead.desktop.tests.utilities
{
    public class PermissionApiTests
    {
        private readonly Faker faker = new Faker();

        [Fact]
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var api = new PermissionApi(faker.Internet.DomainName());
                Assert.NotNull(api.InternetUtility);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanInjectStatusProvider()
        {
            var mock = new ActiveActionInternetStatus();
            var exception = Record.Exception(() =>
            {
                var api = new PermissionApi(
                    faker.Internet.DomainName(), mock);
                Assert.NotNull(api.InternetUtility);
                Assert.Equal(api.InternetUtility, mock);
            });
            Assert.Null(exception);
        }

        private sealed class ActiveActionInternetStatus : IInternetStatus
        {
            public bool GetConnectionStatus()
            {
                return true;
            }
        }
    }
}