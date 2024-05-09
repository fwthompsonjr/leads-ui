using legallead.logging.interfaces;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Infrastructure
{
    public class LoggingInfrastructureTests
    {

        [Theory]
        [InlineData(typeof(ILoggingInfrastructure))]
        [InlineData(typeof(LoggingInfrastructure))]
        [InlineData(typeof(ILoggingService))]
        [InlineData(typeof(Mock<ILoggingService>))]
        public void MockProviderCanGetTypes(Type type)
        {
            var provider = GetProvider();
            var service = provider.GetService(type);
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogCritical(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogCritical("message");
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogDebug(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogDebug("message");
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogError(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var error = new Faker().System.Exception();
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogError(error);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogInformation(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogInformation("message");
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogVerbose(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogVerbose("message");
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogWarning(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogWarning("message");
            });
            Assert.Null(exception);
        }

        private static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            var loggingDb = new Mock<ILoggingService>();

            // add mocks
            services.AddSingleton(loggingDb);
            services.AddSingleton(loggingDb.Object);

            // expose default implementation
            services.AddSingleton<ILoggingInfrastructure>(x =>
            {
                var infra = x.GetRequiredService<ILoggingService>();
                return new LoggingInfrastructure(infra);
            });

            // expose internal implementation as class
            services.AddSingleton(x =>
            {
                var infra = new LoggingInfrastructure();
                return infra;
            });

            return services.BuildServiceProvider();
        }
    }
}