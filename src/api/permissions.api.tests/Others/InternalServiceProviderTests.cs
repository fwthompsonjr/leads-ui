using legallead.permissions.api;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Others
{
    public class InternalServiceProviderTests
    {
        [Fact]
        public void ProviderPropertiesCheck()
        {
            var collection = new ServiceCollection();
            var sut = new InternalServiceProvider(collection);
            Assert.NotNull(sut.ServiceProvider);
        }
    }
}
