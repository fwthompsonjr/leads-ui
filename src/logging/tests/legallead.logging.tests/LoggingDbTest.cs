namespace legallead.logging.tests
{
    public class LoggingDbTest
    {
        [Fact]
        public void DbCanGetConnectionString()
        {
            var connection = LoggingDb.GetConnectionString();
            Assert.False(string.IsNullOrWhiteSpace(connection));
        }

        [Fact]
        public void DbConnectionStringHasExpectedLength()
        {
            var connection = LoggingDb.GetConnectionString();
            var keys = connection.Split(';').ToList();
            Assert.True(keys.Count >= 5, "The actualCount was not greater than five");
        }

        [Theory]
        [InlineData("server")]
        [InlineData("user")]
        [InlineData("password")]
        [InlineData("Database")]
        [InlineData("port")]
        public void DbConnectionHasExpectedKeys(string key)
        {
            var connection = LoggingDb.GetConnectionString();
            var keys = connection.Split(';').ToList();
            var exists = keys.Exists(a => a.StartsWith(key, StringComparison.OrdinalIgnoreCase));
            Assert.True(exists);
        }

        [Theory]
        [InlineData("server")]
        [InlineData("user")]
        [InlineData("password")]
        [InlineData("Database")]
        [InlineData("port")]
        public void DbConnectionHasExpectedKeyValues(string key)
        {
            var connection = LoggingDb.GetConnectionString();
            var keys = connection.Split(';').ToList();
            var item = keys.First(a => a.StartsWith(key, StringComparison.OrdinalIgnoreCase)) ?? "=";
            var itemdata = item.Split('=')[^1];
            Assert.False(string.IsNullOrWhiteSpace(itemdata));
        }
    }
}