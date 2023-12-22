using Bogus;
using legallead.logging.helpers;
using legallead.logging.implementations;
using legallead.logging.interfaces;
using legallead.logging.tests.testobj;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;

namespace legallead.logging.tests
{
    internal static class TestContextProvider
    {
        private static readonly object locker = new();

        public static IServiceProvider GetTestFramework()
        {
            lock (locker)
            {
                var faker = new Faker<TempDto>()
                        .RuleFor(x => x.Id, y => y.IndexFaker)
                        .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(30));
                var collection = new ServiceCollection();
                collection.AddSingleton(s => faker);
                collection.AddSingleton(s => faker.Generate());
                collection.AddSingleton(s => new Mock<ILoggingDbCommand>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<ILoggingDbCommand>>().Object);
                collection.AddSingleton(s => new Mock<IDbConnection>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDbConnection>>().Object);
                collection.AddSingleton(s => new Mock<IDbCommand>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDbCommand>>().Object);
                collection.AddSingleton(s => new Mock<IDataReader>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDataReader>>().Object);
                collection.AddSingleton<TempDtoRepository>();
                collection.AddSingleton(s =>
                {
                    var service = s.GetRequiredService<ILoggingDbCommand>();
                    return new Mock<LoggingDbContext>(service);
                });
                collection.AddSingleton(s =>
                {
                    var service = s.GetRequiredService<Mock<LoggingDbContext>>();
                    return service.Object;
                });
                var provider = collection.BuildServiceProvider();
                var dto = provider.GetRequiredService<TempDto>();
                var contentContextMk = provider.GetRequiredService<Mock<LoggingDbContext>>();
                contentContextMk.SetupGet(g => g.GetCommand).Returns(provider.GetRequiredService<ILoggingDbCommand>());
                contentContextMk.Setup(g => g.CreateConnection()).Returns(provider.GetRequiredService<IDbConnection>());
                var connectionMk = provider.GetRequiredService<Mock<IDbConnection>>();
                connectionMk.Setup(g => g.CreateCommand()).Returns(provider.GetRequiredService<IDbCommand>());
                var commandMk = provider.GetRequiredService<Mock<IDbCommand>>();
                commandMk.Setup(g => g.ExecuteReader()).Returns(provider.GetRequiredService<IDataReader>());
                var readerMk = provider.GetRequiredService<Mock<IDataReader>>();
                readerMk.SetupGet(g => g.FieldCount).Returns(dto.FieldList.Count);
                readerMk.Setup(m => m.GetName(It.Is<int>(i => i == 0))).Returns(dto.FieldList[0]);
                readerMk.Setup(m => m.GetName(It.Is<int>(i => i == 1))).Returns(dto.FieldList[1]);
                return provider;
            }
        }
    }
}