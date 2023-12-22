using legallead.logging.helpers;
using legallead.logging.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.logging.tests.helpers
{
    public class ContentDbContextTests
    {
        [Fact]
        public void ContextCanBeCreated()
        {
            var provider = TestContextProvider.GetTestFramework();
            var command = provider.GetRequiredService<ILoggingDbCommand>();
            var sut = new LoggingDbContext(command);
            Assert.NotNull(sut);
        }

        [Fact]
        public void ContextCanGetCommand()
        {
            var provider = TestContextProvider.GetTestFramework();
            var command = provider.GetRequiredService<ILoggingDbCommand>();
            var sut = new LoggingDbContext(command);
            Assert.NotNull(sut.GetCommand);
        }

        [Fact]
        public void ContextCanGetConnection()
        {
            var provider = TestContextProvider.GetTestFramework();
            var command = provider.GetRequiredService<ILoggingDbCommand>();
            var sut = new LoggingDbContext(command);
            var connected = sut.CreateConnection();
            Assert.NotNull(connected);
        }
    }
}