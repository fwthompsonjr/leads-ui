using legallead.email.services;

namespace legallead.email.tests.services
{
    public class ConnectionStringServiceTest
    {

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1, false)]
        public void ServiceCanGetCredential(int index, bool hasNoException = true)
        {
            var service = new ConnectionStringService(new MockCryptographyService(hasNoException));
            var response = service.GetCredential();
            Assert.NotNull(response);
            if (hasNoException)
            {

                Assert.NotEmpty(response);
                Assert.Equal(2, response.Length);
                Assert.NotEmpty(response[index]);
            }
            else
            {
                Assert.Empty(response);
            }
        }

        [Theory]
        [InlineData("server=")]
        [InlineData("user=")]
        [InlineData("password=")]
        [InlineData("Database=")]
        [InlineData("port=")]
        public void ServiceCanGetConnectionString(string key)
        {
            var service = new ConnectionStringService(new MockCryptographyService());
            var response = service.ConnectionString();
            Assert.False(string.IsNullOrEmpty(response));
            Assert.Contains(key, response);
        }

        private sealed class MockCryptographyService(bool hasNoError = true) : CryptographyService
        {
            private readonly bool canDecrypt = hasNoError;
            private const string _hashPhrase = "this.is.just.a.unit.test";
            private static readonly string[] credential = ["username", "123password456"];

            private static string UserId => credential[0];
            private static string Password => credential[1];

            public override string Decrypt(string input, string key, string vectorBase64)
            {
                if (canDecrypt)
                {
                    var encoded = Encrypt(input, key, out var vector);
                    return base.Decrypt(encoded, _hashPhrase, vector);
                }
                var err = new Faker().System.Exception();
                throw err;
            }

            public override string Encrypt(string input, string key, out string vector)
            {
                var keyvalue = $"{UserId}|{Password}";
                return base.Encrypt(keyvalue, _hashPhrase, out vector);
            }

        }
    }
}
