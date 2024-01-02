using legallead.desktop.entities;
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
        [InlineData(typeof(IInternetStatus))]
        [InlineData(typeof(MenuConfiguration))]
        [InlineData(typeof(IErrorContentProvider))]
        [InlineData(typeof(IUserProfileMapper))]
        [InlineData(typeof(ICopyrightBuilder))]
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