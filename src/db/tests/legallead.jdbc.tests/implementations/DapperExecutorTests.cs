using Dapper;
using legallead.jdbc.helpers;
using Moq;
using Moq.Dapper;
using System.Data.Common;

namespace legallead.jdbc.tests.implementations
{
    public class DapperExecutorTests
    {
        [Fact]
        public async Task ExecutorCanExecuteAsync()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var connection = new Mock<DbConnection>();
                connection.SetupDapperAsync(c => c.ExecuteAsync("", null, null, null, null))
                          .ReturnsAsync(1);
                var executor = new DapperExecutor();
                await executor.ExecuteAsync(connection.Object, "");
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ExecutorCanQueryAsync()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var connection = new Mock<DbConnection>();

                var expected = new[] { 7, 77, 777 };

                connection.SetupDapperAsync(c => c.QueryAsync<int>(It.IsAny<string>(), null, null, null, null))
                          .ReturnsAsync(expected);
                var executor = new DapperExecutor();
                var actual = (await executor.QueryAsync<int>(connection.Object, "")).ToList();
                Assert.Equal(expected.Length, actual.Count);
                Assert.Equivalent(expected, actual);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task ExecutorCanQuerySingleAsync()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var connection = new Mock<DbConnection>();

                var expected = 7;

                connection.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), null, null, null, null))
                          .ReturnsAsync(expected);
                var executor = new DapperExecutor();
                var actual = await executor.QuerySingleOrDefaultAsync<int>(connection.Object, "");
                Assert.Equal(expected, actual);
            });
            Assert.Null(exception);
        }
    }
}