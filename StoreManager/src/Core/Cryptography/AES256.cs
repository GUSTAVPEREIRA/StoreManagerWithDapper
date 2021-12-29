using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Cryptography
{
    public static class Aes256
    {
        private static readonly byte[] SaltBytes = {2, 1, 7, 3, 6, 4, 8, 5};

        public static string EncryptString(string data, string secret)
        {
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(data);
            var passwordBytes = Encoding.UTF8.GetBytes(secret);

            passwordBytes = SHA512.Create().ComputeHash(passwordBytes);
            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);
            var encryptedResult = Convert.ToBase64String(bytesEncrypted);

            return encryptedResult;
        }

        public static string DecryptString(string data, string password)
        {
            var bytesToBeDecrypted = Convert.FromBase64String(data);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA512.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);
            var decryptedResult = Encoding.UTF8.GetString(bytesDecrypted);

            return decryptedResult;
        }


        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            using var ms = new MemoryStream();
            var aes = Aes.Create();

            aes.KeySize = 256;
            aes.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            aes.Mode = CipherMode.CBC;

            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
            cs.Close();

            var encryptedBytes = ms.ToArray();

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            using var ms = new MemoryStream();
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            aes.Mode = CipherMode.CBC;

            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
            cs.Close();

            var decryptedBytes = ms.ToArray();

            return decryptedBytes;
        }
    }
}