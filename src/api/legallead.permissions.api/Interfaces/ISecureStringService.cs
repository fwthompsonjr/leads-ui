namespace legallead.permissions.api.Interfaces
{
    public interface ISecureStringService
    {
        string Encrypt(string plainText, string passPhrase, out string vector);
        string Decrypt(string encodedText, string passPhrase, string vector);
    }
}
