using legallead.desktop.interfaces;
using legallead.desktop.utilities;

namespace legallead.desktop.tests.utilities
{
    public class DesktopCoreServiceProviderTests
    {
        [Fact]
        public void CanGetDesktopServiceProvider()
        {
            var exception = Record.Exception(() =>
            {
                _ = DesktopCoreServiceProvider.Provider;
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(typeof(IContentParser))]
        [InlineData(typeof(IContentHtmlNames))]
        public void CanGetRegisteredType(Type type)
        {
            // DesktopCoreServiceProvider.
            var exception = Record.Exception(() =>
            {
                var provider = DesktopCoreServiceProvider.Provider;
                var sut = provider?.GetService(type);
                Assert.NotNull(sut);
            });
            Assert.Null(exception);
        }
    }
}