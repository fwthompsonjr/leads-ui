using Dapper;
using legallead.email.services;
using legallead.email.tests.connection;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Dapper;
using System.Data;
using System.Data.Common;

namespace legallead.email.tests.services
{
    public class DataCommandServiceTests
    {
        [Fact]
        public async Task ExecutorCanExecuteAsync()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var connection = new Mock<DbConnection>();
                connection.SetupDapperAsync(c => c.ExecuteAsync("", null, null, null, null))
                          .ReturnsAsync(1);
                var executor = new DataCommandService();
                await executor.ExecuteAsync(connection.Object, "");
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ExecutorCanExecuteAsyncWithParameter()
        {
            var executor = new DataCommandService();
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExecutorCanQueryAsync(bool hasParameters)
        {
            var executor = new DataCommandService();
            var provider = TestContextProvider.GetTestFramework();
            var connection = provider.GetRequiredService<IDbConnection>();
            var cmmdMq = provider.GetRequiredService<Mock<IDbCommand>>();
            var readerMq = provider.GetRequiredService<Mock<IDataReader>>();
            var dto = provider.GetRequiredService<TempDto>();
            var dbparmcollection = new Mock<IDataParameterCollection>();
            var dbparm = new Mock<IDbDataParameter>();

            var sql = dto.InsertSQL();
            var parms = dto.InsertParameters();

            cmmdMq.Setup(m => m.ExecuteNonQuery());
            cmmdMq.Setup(m => m.CreateParameter()).Returns(dbparm.Object);
            cmmdMq.SetupGet(m => m.Parameters).Returns(dbparmcollection.Object);
            readerMq.SetupSequence(m => m.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            var result = hasParameters ?
                await executor.QueryAsync<TempDto>(connection, sql, parms) :
                await executor.QueryAsync<TempDto>(connection, sql);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task ExecutorCanQuerySingleOrDefaultAsync(bool hasParameters, bool hasResult)
        {
            var executor = new DataCommandService();
            var provider = TestContextProvider.GetTestFramework();
            var connection = provider.GetRequiredService<IDbConnection>();
            var cmmdMq = provider.GetRequiredService<Mock<IDbCommand>>();
            var readerMq = provider.GetRequiredService<Mock<IDataReader>>();
            var dto = provider.GetRequiredService<TempDto>();
            var dbparmcollection = new Mock<IDataParameterCollection>();
            var dbparm = new Mock<IDbDataParameter>();

            var sql = dto.InsertSQL();
            var parms = dto.InsertParameters();

            cmmdMq.Setup(m => m.ExecuteNonQuery());
            cmmdMq.Setup(m => m.CreateParameter()).Returns(dbparm.Object);
            cmmdMq.SetupGet(m => m.Parameters).Returns(dbparmcollection.Object);
            if (hasResult)
            {
                readerMq.SetupSequence(m => m.Read())
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);
            } 
            else
            {
                readerMq.Setup(m => m.Read()).Returns(false);
            }

            var result = hasParameters ?
                await executor.QuerySingleOrDefaultAsync<TempDto>(connection, sql, parms) :
                await executor.QuerySingleOrDefaultAsync<TempDto>(connection, sql);

            if (hasResult) Assert.NotNull(result);
            else Assert.Null(result);
        }
    }
}
