using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ExTools.Infrastructure
{
    public static class StringCipher
    {
        private const int DERIVATION_ITERATIONS = 1000;
        private const int KEY_SIZE = 256;
        private const string PASS_PHRASE = "ExTools";

        public static string Decrypt(string cipherText)
        {
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);

            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv
                .Take(KEY_SIZE / 8)
                .ToArray();

            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv
                .Skip(KEY_SIZE / 8)
                .Take(KEY_SIZE / 8)
                .ToArray();

            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv
                .Skip(KEY_SIZE / 8 * 2)
                .Take(cipherTextBytesWithSaltAndIv.Length - (KEY_SIZE / 8 * 2))
                .ToArray();

            using Rfc2898DeriveBytes password = new(PASS_PHRASE, saltStringBytes, DERIVATION_ITERATIONS);
            byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);

            using RijndaelManaged symmetricKey = GetSymmetricKey();
            using ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
            using MemoryStream memoryStream = new(cipherTextBytes);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string Encrypt(string plainText)
        {
            byte[] saltStringBytes = Generate256BitsOfRandomEntropy();
            byte[] ivStringBytes = Generate256BitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using Rfc2898DeriveBytes password = new(PASS_PHRASE, saltStringBytes, DERIVATION_ITERATIONS);
            byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);

            using RijndaelManaged symmetricKey = GetSymmetricKey();
            using ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = saltStringBytes;
            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(cipherTextBytes);
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32];
            using (RNGCryptoServiceProvider rngCsp = new())
            {
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        private static RijndaelManaged GetSymmetricKey() => new()
        {
            BlockSize = 256,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7
        };
    }
}