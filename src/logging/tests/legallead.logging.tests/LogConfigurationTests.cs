using Bogus;

namespace legallead.logging.tests
{
    public class LogConfigurationTests
    {
        private readonly Faker faker = new();
        private readonly LogConfiguration logConfiguration = new();

        [Fact]
        public void LogConfigurationHasALoggingLevel()
        {
            var exception = Record.Exception(() => { _ = logConfiguration.LogLevel; });
            Assert.Null(exception);
        }

        [Fact]
        public void LogConfigurationCanSetLevel()
        {
            var exception = Record.Exception(() =>
            {
                var item = faker.PickRandom<LogConfigurationLevel>();
                logConfiguration.SetLoggingLevel(item);
            });
            Assert.Null(exception);
        }
    }
}