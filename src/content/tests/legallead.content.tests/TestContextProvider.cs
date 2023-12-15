using Bogus;
using legallead.content.helpers;
using legallead.content.implementations;
using legallead.content.interfaces;
using legallead.content.tests.testobj;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.content.tests
{
    internal static class TestContextProvider
    {
        private static readonly object locker = new();

        public static IServiceProvider GetTestFramework()
        {
            lock (locker)
            {
                var faker = new Faker<TempDto>()
                        .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
                        .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(30));
                var collection = new ServiceCollection();
                collection.AddSingleton(s => faker);
                collection.AddSingleton(s => faker.Generate());
                collection.AddSingleton(s => new Mock<IContentDbCommand>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IContentDbCommand>>().Object);
                collection.AddSingleton(s => new Mock<IDbConnection>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDbConnection>>().Object);
                collection.AddSingleton(s => new Mock<IDbCommand>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDbCommand>>().Object);
                collection.AddSingleton(s => new Mock<IDataReader>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDataReader>>().Object);
                collection.AddSingleton<TempDtoRepository>();
                collection.AddSingleton(s =>
                {
                    var service = s.GetRequiredService<IContentDbCommand>();
                    return new Mock<ContentDbContext>(service);
                });
                collection.AddSingleton(s =>
                {
                    var service = s.GetRequiredService<Mock<ContentDbContext>>();
                    return service.Object;
                });
                collection.AddSingleton<IWebContentLineRepository, WebContentLineRepository>();
                collection.AddSingleton<IWebContentRepository, WebContentRepository>();
                var provider = collection.BuildServiceProvider();
                var dto = provider.GetRequiredService<TempDto>();
                var contentContextMk = provider.GetRequiredService<Mock<ContentDbContext>>();
                contentContextMk.SetupGet(g => g.GetCommand).Returns(provider.GetRequiredService<IContentDbCommand>());
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