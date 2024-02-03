using Bogus;
using legallead.logging.interfaces;
using legallead.search.api.Services;
using Moq;

namespace legallead.search.api.tests.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods",
        Justification = "Ignoring naming convention on purpose for unit tests")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Cube",
        "S3236:Caller information arguments should not be provided explicitly",
        Justification = "This behavior allows for a generic mock execution")]
    public class LoggingRepositoryTests
    {
        private static readonly Faker faker = new();
        [Fact]
        public void RepoHasClassContext()
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object)
            {
                ClassContext = attribute
            };
            Assert.Equal(attribute, sut.ClassContext);
        }

        [Fact]
        public async Task RepoCanLogCritical()
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object);
            mock.Setup(m => m.LogCritical(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            await sut.LogCritical(attribute);
            mock.Verify(m => m.LogCritical(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async Task RepoCanLogDebug()
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object);
            mock.Setup(m => m.LogDebug(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            await sut.LogDebug(attribute);
            mock.Verify(m => m.LogDebug(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]

        public async Task RepoCanLogError()
        {
            var attribute = faker.System.Exception();
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object);
            mock.Setup(m => m.LogError(It.IsAny<Exception>(), It.IsAny<int>(), It.IsAny<string>()));
            await sut.LogError(attribute);
            mock.Verify(m => m.LogError(It.IsAny<Exception>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async Task RepoCanLogInformation()
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object);
            mock.Setup(m => m.LogInformation(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            await sut.LogInformation(attribute);
            mock.Verify(m => m.LogInformation(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async Task RepoCanLogVerbose()
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object);
            mock.Setup(m => m.LogVerbose(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            await sut.LogVerbose(attribute);
            mock.Verify(m => m.LogVerbose(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async Task RepoCanLogWarning()
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var mock = new Mock<ILoggingService>();
            var sut = new LoggingRepository(mock.Object);
            mock.Setup(m => m.LogWarning(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            await sut.LogWarning(attribute);
            mock.Verify(m => m.LogWarning(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Theory]
        [InlineData("LogCritical")]
        [InlineData("LogDebug")]
        [InlineData("LogError")]
        [InlineData("LogInformation")]
        [InlineData("LogVerbose")]
        [InlineData("LogWarning")]
        public async Task RepoLogWithoutInfrastructure(string methodName)
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var exception = faker.System.Exception();
            var sut = new LoggingRepository(null);
            var failure = await Record.ExceptionAsync(async () =>
            {
                if (methodName == "LogCritical") await sut.LogCritical(attribute);
                if (methodName == "LogDebug") await sut.LogDebug(attribute);
                if (methodName == "LogError") await sut.LogError(exception);
                if (methodName == "LogError") await sut.LogError(exception, "", "");
                if (methodName == "LogInformation") await sut.LogInformation(attribute);
                if (methodName == "LogInformation") await sut.LogInformation(attribute, "", "");
                if (methodName == "LogVerbose") await sut.LogVerbose(attribute);
                if (methodName == "LogWarning") await sut.LogWarning(attribute);
            });
            Assert.Null(failure);
        }
        [Theory]
        [InlineData("LogCritical")]
        [InlineData("LogDebug")]
        [InlineData("LogError")]
        [InlineData("LogInformation")]
        [InlineData("LogVerbose")]
        [InlineData("LogWarning")]
        public async Task RepoLogServiceException(string methodName)
        {
            var attribute = faker.Random.AlphaNumeric(10);
            var exception = faker.System.Exception();
            var infra = new Mock<ILoggingService>();

            infra.Setup(m => m.LogCritical(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            infra.Setup(m => m.LogDebug(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            infra.Setup(m => m.LogInformation(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            infra.Setup(m => m.LogVerbose(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            infra.Setup(m => m.LogWarning(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            infra.Setup(m => m.LogError(It.IsAny<Exception>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            infra.Setup(m => m.LogInformation(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception); 
            infra.Setup(m => m.LogError(
            It.IsAny<Exception>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(exception);
            var sut = new LoggingRepository(infra.Object);
            var failure = await Record.ExceptionAsync(async () =>
            {
                if (methodName == "LogCritical") await sut.LogCritical(attribute);
                if (methodName == "LogDebug") await sut.LogDebug(attribute);
                if (methodName == "LogError") await sut.LogError(exception);
                if (methodName == "LogError") await sut.LogError(exception, "", "");
                if (methodName == "LogInformation") await sut.LogInformation(attribute);
                if (methodName == "LogInformation") await sut.LogInformation(attribute, "", "");
                if (methodName == "LogVerbose") await sut.LogVerbose(attribute);
                if (methodName == "LogWarning") await sut.LogWarning(attribute);
            });
            Assert.Null(failure);
        }
    }
}
