using System.Security.Cryptography;
using System.Text;

namespace legallead.permissions.api.Services
{
    public class SecureStringService : ISecureStringService
    {
        public string Decrypt(string encodedText, string passPhrase, string vector)
        {
            var keyBase64 = GetKeyBase64(passPhrase);
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.IV = Convert.FromBase64String(vector);


            // Create decryptor object
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

            byte[] cipher = Convert.FromBase64String(encodedText);

            //Decryption will be done in a memory stream through a CryptoStream object
            using MemoryStream ms = new(cipher);
            using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }

        public string Encrypt(string plainText, string passPhrase, out string vector)
        {
            var keyBase64 = GetKeyBase64(passPhrase);
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.GenerateIV();

            vector = Convert.ToBase64String(aesAlgorithm.IV);
            // Create encryptor object
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

            byte[] encryptedData;

            //Encryption will be done in a memory stream through a CryptoStream object
            using (MemoryStream ms = new())
            {
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs)) sw.Write(plainText);
                encryptedData = ms.ToArray();
            }

            return Convert.ToBase64String(encryptedData);
        }

        public static string GetKeyBase64(string passPhrase)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(passPhrase);
            return Convert.ToBase64String(inputArray, 0, inputArray.Length);
        }
        public static string FromBase64(string encoded)
        {
            var key = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(key);
        }
    }
}
