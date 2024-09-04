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
        public async Task ServiceCanLogCriticalAsync(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogCriticalAsync("message");
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogDebugAsync(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogDebugAsync("message");
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogErrorAsync(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var error = new Faker().System.Exception();
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogErrorAsync(error);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogInformationAsync(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogInformationAsync("message");
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogVerboseAsync(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogVerboseAsync("message");
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanLogWarningAsync(bool useConcreteClass)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                ILoggingInfrastructure service =
                    useConcreteClass ?
                    provider.GetRequiredService<LoggingInfrastructure>() :
                    provider.GetRequiredService<ILoggingInfrastructure>();
                await service.LogWarningAsync("message");
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