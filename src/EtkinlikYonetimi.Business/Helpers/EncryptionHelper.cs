using System.Security.Cryptography;
using System.Text;

namespace EtkinlikYonetimi.Business.Helpers
{
    /// <summary>
    /// Provides encryption and decryption functionality for passwords
    /// </summary>
    public static class EncryptionHelper
    {
        // Note: In production, this should be moved to configuration (appsettings.json or environment variables)
        private const string DefaultEncryptionKey = "EtkinlikYonetimi2024!";

        /// <summary>
        /// Encrypts a plain text password
        /// </summary>
        /// <param name="password">The plain text password to encrypt</param>
        /// <param name="encryptionKey">Optional encryption key. Uses default if not provided.</param>
        /// <returns>Base64 encoded encrypted password</returns>
        public static string EncryptPassword(string password, string? encryptionKey = null)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            var keyToUse = encryptionKey ?? DefaultEncryptionKey;
            
            using var aes = Aes.Create();
            var key = PrepareEncryptionKey(keyToUse);
            aes.Key = key;
            aes.GenerateIV();

            var iv = aes.IV;
            var encrypted = EncryptStringToBytes(password, aes.Key, iv);
            
            // Combine IV and encrypted data for storage
            var result = CombineIvAndEncryptedData(iv, encrypted);
            
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts an encrypted password
        /// </summary>
        /// <param name="encryptedPassword">The Base64 encoded encrypted password</param>
        /// <param name="encryptionKey">Optional encryption key. Uses default if not provided.</param>
        /// <returns>The decrypted plain text password</returns>
        public static string DecryptPassword(string encryptedPassword, string? encryptionKey = null)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                return string.Empty;

            try
            {
                var keyToUse = encryptionKey ?? DefaultEncryptionKey;
                var fullCipher = Convert.FromBase64String(encryptedPassword);
                
                using var aes = Aes.Create();
                var key = PrepareEncryptionKey(keyToUse);
                aes.Key = key;
                
                // Extract IV and cipher data
                var (iv, cipher) = ExtractIvAndCipherData(fullCipher, aes.BlockSize / 8);
                aes.IV = iv;
                
                return DecryptStringFromBytes(cipher, aes.Key, aes.IV);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Verifies if a plain text password matches an encrypted password
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="encryptedPassword">The encrypted password to compare against</param>
        /// <param name="encryptionKey">Optional encryption key. Uses default if not provided.</param>
        /// <returns>True if passwords match, false otherwise</returns>
        public static bool VerifyPassword(string password, string encryptedPassword, string? encryptionKey = null)
        {
            var decryptedPassword = DecryptPassword(encryptedPassword, encryptionKey);
            return password == decryptedPassword;
        }

        /// <summary>
        /// Prepares the encryption key by padding or truncating to 32 bytes
        /// </summary>
        /// <param name="key">The original key</param>
        /// <returns>A 32-byte key suitable for AES-256</returns>
        private static byte[] PrepareEncryptionKey(string key)
        {
            return Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        }

        /// <summary>
        /// Combines IV and encrypted data into a single byte array
        /// </summary>
        /// <param name="iv">The initialization vector</param>
        /// <param name="encrypted">The encrypted data</param>
        /// <returns>Combined byte array</returns>
        private static byte[] CombineIvAndEncryptedData(byte[] iv, byte[] encrypted)
        {
            var result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);
            return result;
        }

        /// <summary>
        /// Extracts IV and cipher data from combined byte array
        /// </summary>
        /// <param name="fullCipher">The combined byte array</param>
        /// <param name="ivLength">The length of the IV</param>
        /// <returns>Tuple containing IV and cipher data</returns>
        private static (byte[] iv, byte[] cipher) ExtractIvAndCipherData(byte[] fullCipher, int ivLength)
        {
            var iv = new byte[ivLength];
            var cipher = new byte[fullCipher.Length - ivLength];
            
            Buffer.BlockCopy(fullCipher, 0, iv, 0, ivLength);
            Buffer.BlockCopy(fullCipher, ivLength, cipher, 0, cipher.Length);
            
            return (iv, cipher);
        }

        /// <summary>
        /// Encrypts a string to bytes using AES encryption
        /// </summary>
        /// <param name="plainText">The plain text to encrypt</param>
        /// <param name="key">The encryption key</param>
        /// <param name="iv">The initialization vector</param>
        /// <returns>Encrypted bytes</returns>
        /// <exception cref="ArgumentNullException">Thrown when parameters are null or empty</exception>
        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);
            
            swEncrypt.Write(plainText);
            
            return msEncrypt.ToArray();
        }

        /// <summary>
        /// Decrypts bytes to string using AES decryption
        /// </summary>
        /// <param name="cipherText">The encrypted bytes</param>
        /// <param name="key">The decryption key</param>
        /// <param name="iv">The initialization vector</param>
        /// <returns>Decrypted string</returns>
        /// <exception cref="ArgumentNullException">Thrown when parameters are null or empty</exception>
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }
    }
}