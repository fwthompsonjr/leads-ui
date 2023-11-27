using Npgsql;

namespace legallead.jdbc.tests
{
    public class GetConnectionStringTest
    {
        [Fact]
        public void GetConnectionIsNotNull()
        {
            var response = RemoteData.GetConnectionString();
            Assert.NotNull(response);
        }

        [Fact]
        public void GetConnectionAliasIsNotNull()
        {
            var response = RemoteData.GetConnectionAlias();
            Assert.NotNull(response);
        }

        [Fact]
        public void GetConnectionPostGreIsNotNull()
        {
            var response = RemoteData.GetPostGreString();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CanOpenPgresConnection()
        {
            var connectionString = RemoteData.GetPostGreString();
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            Assert.NotNull(dataSource);
            // fetch data
            await using var cmmd = dataSource.CreateCommand("SELECT * FROM APPLICATIONS WHERE 1 = 2");
            await using var reader = await cmmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}