using legallead.email.entities;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text;

namespace legallead.email.tests.connection
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
                collection.AddSingleton(s => new Mock<IDbConnection>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDbConnection>>().Object);
                collection.AddSingleton(s => new Mock<IDbCommand>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDbCommand>>().Object);
                collection.AddSingleton(s => new Mock<IDataReader>());
                collection.AddSingleton(s => s.GetRequiredService<Mock<IDataReader>>().Object);
                var provider = collection.BuildServiceProvider();
                var dto = provider.GetRequiredService<TempDto>();
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
        public static Dapper.DynamicParameters InsertParameters(this BaseDto obj)
        {
            var parms = new Dapper.DynamicParameters();
            var fields = obj.FieldList;
            fields.ForEach(f => parms.Add(f, obj[f]));
            return parms;
        }
        public static string InsertSQL<T>(this T obj, string delimiter = "@")
            where T : BaseDto
        {
            var builder = new StringBuilder($"INSERT INTO [{obj.TableName}] {Environment.NewLine} ( {Environment.NewLine}");
            var fields = new StringBuilder();
            var values = new StringBuilder();
            // exclude any fields without a value
            var list = obj.FieldList;
            list.ForEach(f =>
            {
                // when not the first item
                var comma = f == list[0] ? "    " : "   ,";
                fields.AppendLine($"{comma}[{f}]");
                values.AppendLine($"{comma}{delimiter}{f}");
            });
            builder.Append(fields);
            builder.AppendLine(" ) VALUES ( ");
            builder.Append(values);
            builder.AppendLine(" );");

            return builder.ToString();
        }
    }

}
