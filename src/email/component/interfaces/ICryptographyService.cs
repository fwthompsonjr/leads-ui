namespace legallead.email.services
{
    internal interface ICryptographyService
    {
        string Decrypt(string input, string key, string vectorBase64);
        string Encrypt(string input, string key, out string vector);
    }
}