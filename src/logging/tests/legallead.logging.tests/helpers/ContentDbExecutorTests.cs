using Bogus;
using legallead.logging.extensions;
using legallead.logging.helpers;
using legallead.logging.tests.testobj;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;

namespace legallead.logging.tests.helpers
{
    public class ContentDbExecutorTests
    {
        [Fact]
        public void ExecutorCanBeCreated()
        {
            var executor = new LoggingDbExecutor();
            Assert.NotNull(executor);
        }

        [Fact]
        public async Task ExecutorCanExecuteAsync()
        {
            var executor = new LoggingDbExecutor();
            var provider = TestContextProvider.GetTestFramework();
            var connection = provider.GetRequiredService<IDbConnection>();
            var cmmdMq = provider.GetRequiredService<Mock<IDbCommand>>();
            var dbparm = new Mock<IDbDataParameter>();
            var sql = new Faker().Random.AlphaNumeric(25);

            cmmdMq.Setup(m => m.ExecuteNonQuery());
            cmmdMq.Setup(m => m.CreateParameter()).Returns(dbparm.Object);
            await executor.ExecuteAsync(connection, sql);
            cmmdMq.Verify(m => m.ExecuteNonQuery());
        }

        [Fact]
        public async Task ExecutorCanExecuteAsyncWithParameter()
        {
            var executor = new LoggingDbExecutor();
            var provider = TestContextProvider.GetTestFramework();
            var connection = provider.GetRequiredService<IDbConnection>();
            var cmmdMq = provider.GetRequiredService<Mock<IDbCommand>>();
            var dto = provider.GetRequiredService<TempDto>();
            var dbparmcollection = new Mock<IDataParameterCollection>();
            var dbparm = new Mock<IDbDataParameter>();

            var sql = dto.InsertSQL();
            var parms = dto.InsertParameters();

            cmmdMq.Setup(m => m.ExecuteNonQuery());
            cmmdMq.Setup(m => m.CreateParameter()).Returns(dbparm.Object);
            cmmdMq.SetupGet(m => m.Parameters).Returns(dbparmcollection.Object);
            await executor.ExecuteAsync(connection, sql, parms);
            cmmdMq.Verify(m => m.ExecuteNonQuery());
        }

        [Fact]
        public async Task ExecutorCanQueryAsync()
        {
            var temp = new TempExecutor();
            var executor = temp.Executor;
            var connection = temp.Connection;
            var sql = temp.Statement;
            var result = await executor.QueryAsync<TempDto>(connection, sql);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task ExecutorCanQuerySingleAsync()
        {
            var temp = new TempExecutor();
            var executor = temp.Executor;
            var connection = temp.Connection;
            var sql = temp.Statement;
            var dto = temp.Dto;
            var result = await executor.QuerySingleOrDefaultAsync<TempDto>(connection, sql);
            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
        }

        [Fact]
        public async Task ExecutorCanQuerySingleAsyncWithParms()
        {
            var temp = new TempExecutor();
            var executor = temp.Executor;
            var connection = temp.Connection;
            var sql = temp.Statement;
            var dto = temp.Dto;
            var parms = temp.Parameters;

            var result = await executor.QuerySingleOrDefaultAsync<TempDto>(connection, sql, parms);
            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
        }

        [Fact]
        public async Task ExecutorCanQuerySingleAsyncWithNoResult()
        {
            var executor = new LoggingDbExecutor();
            var provider = TestContextProvider.GetTestFramework();
            var connection = provider.GetRequiredService<IDbConnection>();
            var cmmdMq = provider.GetRequiredService<Mock<IDbCommand>>();
            var reader = provider.GetRequiredService<Mock<IDataReader>>();
            var dto = provider.GetRequiredService<TempDto>();
            var dbparmcollection = new Mock<IDataParameterCollection>();
            var dbparm = new Mock<IDbDataParameter>();

            var sql = dto.SelectSQL();

            cmmdMq.Setup(m => m.ExecuteReader()).Returns(reader.Object);
            cmmdMq.Setup(m => m.CreateParameter()).Returns(dbparm.Object);
            cmmdMq.SetupGet(m => m.Parameters).Returns(dbparmcollection.Object);

            reader.Setup(m => m.Read()).Returns(false);

            var result = await executor.QuerySingleOrDefaultAsync<TempDto>(connection, sql);
            Assert.Null(result);
        }

        private sealed class TempExecutor
        {
            public LoggingDbExecutor Executor { get; private set; }
            public TempDto Dto { get; private set; }
            public IDbConnection Connection { get; private set; }
            public string Statement { get; private set; }
            public Dapper.DynamicParameters Parameters { get; private set; }

            public TempExecutor()
            {
                var executor = new LoggingDbExecutor();
                var provider = TestContextProvider.GetTestFramework();
                var connection = provider.GetRequiredService<IDbConnection>();
                var cmmdMq = provider.GetRequiredService<Mock<IDbCommand>>();
                var reader = provider.GetRequiredService<Mock<IDataReader>>();
                var dto = provider.GetRequiredService<TempDto>();
                var dbparmcollection = new Mock<IDataParameterCollection>();
                var dbparm = new Mock<IDbDataParameter>();

                var sql = dto.SelectSQL();
                var parms = dto.SelectParameters();

                cmmdMq.Setup(m => m.ExecuteReader()).Returns(reader.Object);
                cmmdMq.Setup(m => m.CreateParameter()).Returns(dbparm.Object);
                cmmdMq.SetupGet(m => m.Parameters).Returns(dbparmcollection.Object);

                reader.SetupSequence(m => m.Read())
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

                reader.SetupGet(m => m.FieldCount).Returns(dto.FieldList.Count);
                reader.Setup(m => m.GetName(It.Is<int>(i => i == 0))).Returns(dto.FieldList[0]);
                reader.Setup(m => m.GetName(It.Is<int>(i => i == 1))).Returns(dto.FieldList[1]);
                reader.Setup(m => m[It.Is<int>(i => i == 0)]).Returns(dto[0] ?? new());
                reader.Setup(m => m[It.Is<int>(i => i == 1)]).Returns(dto[1] ?? new());

                Executor = executor;
                Dto = dto;
                Connection = connection;
                Statement = sql;
                Parameters = parms;
            }
        }
    }
}