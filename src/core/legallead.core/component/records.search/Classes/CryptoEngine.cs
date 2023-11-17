using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

namespace legallead.records.search.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Security",
        "CA5350:Do Not Use Weak Cryptographic Algorithms",
        Justification = "TripleDES is used for simplicity and lack of sensitivity of the data.")]
    public static class CryptoEngine
    {
        public static string Encrypt(string input, string key, out string vector)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(key);
            var key64 = Convert.ToBase64String(inputArray, 0, inputArray.Length);
            return EncryptData(input, key64, out vector);
        }

        public static string Decrypt(string input, string key, string vectorBase64)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(key);
            var key64 = Convert.ToBase64String(inputArray, 0, inputArray.Length);
            return DecryptData(input, key64, vectorBase64);
        }


        private static string EncryptData(string plainText, string keyBase64, out string vectorBase64)
        {
            using Aes aesAlgorithm = Aes.Create();
            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.GenerateIV();

            vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);
            // Create encryptor object
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

            byte[] encryptedData;

            //Encryption will be done in a memory stream through a CryptoStream object
            using (MemoryStream ms = new())
            {
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(plainText);
                }
                encryptedData = ms.ToArray();
            }

            return Convert.ToBase64String(encryptedData);
        }


        private static string DecryptData(string cipherText, string keyBase64, string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

                Console.WriteLine($"Aes Cipher Mode : {aesAlgorithm.Mode}");
                Console.WriteLine($"Aes Padding Mode: {aesAlgorithm.Padding}");
                Console.WriteLine($"Aes Key Size : {aesAlgorithm.KeySize}");
                Console.WriteLine($"Aes Block Size : {aesAlgorithm.BlockSize}");


                // Create decryptor object
                ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                byte[] cipher = Convert.FromBase64String(cipherText);

                //Decryption will be done in a memory stream through a CryptoStream object
                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}