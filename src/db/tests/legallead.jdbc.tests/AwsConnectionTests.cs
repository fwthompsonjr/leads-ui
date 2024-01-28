using legallead.jdbc.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.tests
{
    public class AwsConnectionTests
    {
        [Theory]
        [InlineData("local")]
        [InlineData("test")]
        [InlineData("production")]
        [InlineData("undefined")]
        public void AwsCanGenerateConnectionString(string environment)
        {
            var actual = AwsData.GetPostGreString(environment);
            var expected = environment.Equals("undefined");
            Assert.Equal(expected, string.IsNullOrEmpty(actual));
        }

        [Theory]
        [InlineData("app", "local")]
        [InlineData("app", "test")]
        [InlineData("app", "production")]
        [InlineData("error", "local")]
        [InlineData("error", "test")]
        [InlineData("error", "production")]
        public void AwsCanGenerateDatabaseString(string database, string environment)
        {
            var actual = AwsData.GetPostGreString(environment, database);
            Assert.False(string.IsNullOrEmpty(actual));
        }

        [Theory]
        [InlineData("app", "local")]
        [InlineData("app", "test")]
        [InlineData("app", "production")]
        [InlineData("error", "local")]
        [InlineData("error", "test")]
        public void AwsCanCreateConnection(string database, string environment)
        {
            var executor = new DapperExecutor();
            var initializer = new DataInitializer(environment, database);
            var context = new DataContext(executor, initializer, environment, database);
            var actual = context.CreateConnection();
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("app", "local")]
        [InlineData("app", "test")]
        [InlineData("app", "production")]
        [InlineData("error", "local")]
        [InlineData("error", "test")]
        public void AwsCanOpenConnection(string database, string environment)
        {
            var ex = Record.Exception(() =>
            {
                var executor = new DapperExecutor();
                var initializer = new DataInitializer(environment, database);
                var context = new DataContext(executor, initializer, environment, database);
                using var actual = context.CreateConnection();
                actual.Open();
                var command = actual.CreateCommand();
                command.CommandText = "SELECT 1";
                command.ExecuteScalar();
            });
            Assert.Null(ex);
        }
    }
}
