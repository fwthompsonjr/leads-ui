using legallead.email.services;

namespace legallead.email.tests.services
{
    public class DataConnectionServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var service = new DataConnectionService(GetInfrastructure());
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanCreateConnection()
        {
            var service = new DataConnectionService(GetInfrastructure());
            var connect = service.CreateConnection();
            Assert.NotNull(connect);
        }

        private static IConnectionStringService GetInfrastructure()
        {
            var infra = new MockCryptographyService();
            return new ConnectionStringService(infra);
        }

        private sealed class MockCryptographyService : CryptographyService
        {
            private const string _hashPhrase = "this.is.just.a.unit.test";
            private static readonly string[] credential = ["username", "123password456"];

            private static string UserId => credential[0];
            private static string Password => credential[1];

            public override string Decrypt(string input, string key, string vectorBase64)
            {
                var encoded = Encrypt(input, key, out var vector);
                return base.Decrypt(encoded, _hashPhrase, vector);
            }

            public override string Encrypt(string input, string key, out string vector)
            {
                var keyvalue = $"{UserId}|{Password}";
                return base.Encrypt(keyvalue, _hashPhrase, out vector);
            }

        }
    }
}
