using Bogus;

namespace legallead.content.tests
{
    public class CryptoContentTests
    {
        private readonly Faker faker = new();
        private static readonly string PassPhrase = "legal.lead.test.passcode";

        [Fact]
        public void CryptoCanEncrypt()
        {
            var phrase = faker.Hacker.Phrase();
            var encrypted = CryptoContent.Encrypt(phrase, PassPhrase, out var vector);
            Assert.False(string.IsNullOrWhiteSpace(encrypted));
            Assert.False(string.IsNullOrWhiteSpace(vector));
            Assert.NotEqual(phrase, vector);
        }

        [Fact]
        public void CryptoCanDecrypt()
        {
            var phrase = faker.Hacker.Phrase();
            var encrypted = CryptoContent.Encrypt(phrase, PassPhrase, out var vector);
            var decoded = CryptoContent.Decrypt(encrypted, PassPhrase, vector);
            Assert.Equal(phrase, decoded);
        }
    }
}