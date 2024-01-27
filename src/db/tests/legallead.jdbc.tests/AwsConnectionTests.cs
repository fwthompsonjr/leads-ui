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
    }
}
