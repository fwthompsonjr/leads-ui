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
    }
}