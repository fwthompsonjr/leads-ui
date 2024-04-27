using legallead.email.services;

namespace legallead.email.tests.services
{
    public class SettingsServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SettingsService();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanGetSettings()
        {
            var exception = Record.Exception(() =>
            {
                var test = new SettingsService();
                Assert.NotNull(test.GetSettings);
            });
            Assert.Null(exception);
        }
    }
}
