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
        public void CanOpenRemoteConnection()
        {
            IDbConnection? conn = null;
            try
            {
                var connectionString = GetPostGreString("ForceRemote");
                using var dataSource = GetConnection(connectionString);
                Assert.NotNull(dataSource);
                conn = dataSource;
                conn.Open();
                // fetch data
                using var cmmd = conn.CreateCommand();
                cmmd.CommandText = "SELECT * FROM APPLICATIONS WHERE 1 = 2";
                using var reader = cmmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                }
                conn.Close();
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
                conn?.Dispose();
            }
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

        private static IDbConnection GetConnection(string connectionString)
        {
            var connectionType = GetConnectionType();
            switch (connectionType)
            {
                case DbConnectionType.MySQL:
                    return new MySqlConnection(connectionString);
                case DbConnectionType.PostGres:
                    return new NpgsqlConnection(connectionString);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}