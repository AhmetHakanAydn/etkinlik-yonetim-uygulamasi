using System;
using System.Security.Cryptography;
using System.Text;

namespace EtkinlikYonetimi.Business.Helpers
{
    public static class EncryptionHelper
    {
        private const string EncryptionKey = "EtkinlikYonetimi2024!"; // In production, this should be in configuration

        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (var aes = Aes.Create())
            {
                var key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                aes.Key = key;
                aes.GenerateIV();

                var iv = aes.IV;
                var encrypted = EncryptStringToBytes(password, aes.Key, iv);
                
                // Combine IV and encrypted data
                var result = new byte[iv.Length + encrypted.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);
                
                return Convert.ToBase64String(result);
            }
        }

        public static string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                return string.Empty;

            try
            {
                var fullCipher = Convert.FromBase64String(encryptedPassword);
                
                using (var aes = Aes.Create())
                {
                    var key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                    aes.Key = key;
                    
                    // Extract IV
                    var iv = new byte[aes.BlockSize / 8];
                    var cipher = new byte[fullCipher.Length - iv.Length];
                    
                    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
                    
                    aes.IV = iv;
                    
                    return DecryptStringFromBytes(cipher, aes.Key, aes.IV);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool VerifyPassword(string password, string encryptedPassword)
        {
            var decryptedPassword = DecryptPassword(encryptedPassword);
            return password == decryptedPassword;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            byte[] encrypted;

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            string plaintext = null!;

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var msDecrypt = new System.IO.MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }

            return plaintext;
        }
    }
}