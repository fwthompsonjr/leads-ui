using Bogus;
using legallead.logging.entities;
using legallead.logging.implementations;
using legallead.logging.interfaces;

namespace legallead.logging.tests
{
    public class LoggingServiceTests
    {
        private readonly Faker faker = new();

        [Fact]
        public void ServiceCanBeCreated()
        {
            var service = new LoggingService();
            Assert.NotNull(service);
        }

        [Fact]
        public async Task SutCanWriteError()
        {
            var service = new LoggingService(null, new MockRepository());
            var ex = faker.System.Exception();
            var exception = await Record.ExceptionAsync(async () => { _ = await service.LogError(ex); });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SutCanWriteCritical()
        {
            var service = new LoggingService(null, new MockRepository());
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () => { _ = await service.LogCritical(ex); });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SutCanWriteWarning()
        {
            var service = new LoggingService(null, new MockRepository());
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () => { _ = await service.LogWarning(ex); });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SutCanWriteInformation()
        {
            var service = new LoggingService(null, new MockRepository());
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () => { _ = await service.LogInformation(ex); });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SutCanWriteDebug()
        {
            var service = new LoggingService(null, new MockRepository());
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () => { _ = await service.LogDebug(ex); });
            Assert.Null(exception);
        }

        [Fact]
        public async Task SutCanWriteVerbose()
        {
            var service = new LoggingService(null, new MockRepository());
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () => { _ = await service.LogVerbose(ex); });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ServiceCanSerializeInsertParameters()
        {
            var repo = new MockRepository();
            var service = new LoggingService(null, repo);
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () =>
            {
                var dto = await service.LogVerbose(ex);
                await repo.Insert(dto);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ServiceCanSerializeQueryParameters()
        {
            var repo = new MockRepository();
            var service = new LoggingService(null, repo);
            var ex = faker.System.Exception().Message;
            var exception = await Record.ExceptionAsync(async () =>
            {
                var dto = await service.LogVerbose(ex);
                var query = new LogQueryModel
                {
                    Id = 1,
                    ClassName = dto.ClassName
                };
                await repo.Query(query);
            });
            Assert.Null(exception);
        }

        private sealed class MockRepository : ILogContentRepository
        {
            public Task Insert(LogInsertModel dto)
            {
                _ = LoggingService.GetInsertParameters(dto);
                return Task.CompletedTask;
            }

            public Task InsertChild(LogContentDetailDto dto)
            {
                return Task.CompletedTask;
            }

            public async Task<IEnumerable<VwLogDto>> Query(LogQueryModel query)
            {
                _ = LoggingService.GetQueryParameters(query);
                var result = await Task.Run(() => new List<VwLogDto>());
                return result;
            }
        }
    }
}