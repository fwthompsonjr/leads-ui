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
    }
}