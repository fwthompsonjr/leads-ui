using MySqlConnector;
using Npgsql;
using System.Data;
using static legallead.jdbc.RemoteData;

namespace legallead.jdbc.tests
{
    public class GetConnectionStringTest
    {
        [Fact]
        public void GetConnectionPostGreIsNotNull()
        {
            var response = GetPostGreString("ForceRemote");
            Assert.NotNull(response);
        }

        [Fact]
        public void GetLocalConnectionPostGreIsNotNull()
        {
            var response = LocalData.GetPostGreString();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CanOpenLocalPgresConnection()
        {
            var connectionString = LocalData.GetPostGreString();
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