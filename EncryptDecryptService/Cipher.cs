using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EncryptDecryptService.Buffers;
using EncryptDecryptService.Utils;

namespace EncryptDecryptService
{
    public class Cipher
    {
        public Cipher() : this(HashAlgorithm.SHA256)
        { }

        public Cipher(HashAlgorithm hashAlgorithm)
        {
            HashAlgorithm = hashAlgorithm;
            SaltBuffer = CreateRandomByteBuffer();
            InitBuffer = CreateRandomByteBuffer();
        }

        public HashAlgorithm HashAlgorithm { get; }

        private byte[] SaltBuffer { get; set; }

        private byte[] InitBuffer { get; set; }

        /// <summary>
        /// Encrypts the given text by using the given password
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="password">Encryption password</param>
        /// <returns>Encrypted text in Base64 string format</returns>
        public string EncryptText(string plainText, string password)
        {
            var plainTextBuffer = Encoding.UTF8.GetBytes(plainText);
            var encryptedBuffer = EncryptDecryptText(plainTextBuffer, password, true);
            var textEncryptBuffer = new TextEncryptBuffer(password, InitBuffer, SaltBuffer, encryptedBuffer);

            return Convert.ToBase64String(textEncryptBuffer.CombineBuffer());
        }

        /// <summary>
        /// Decrypts the given text by using the given password
        /// </summary>
        /// <param name="encryptedText">Text to be decrypted</param>
        /// <param name="password">Decryption password</param>
        /// <returns>Decrypted text in string format</returns>
        public string DecryptText(string encryptedText, string password)
        {
            var encryptedCombinedBuffer = Convert.FromBase64String(encryptedText);
            var textDecryptBuffer = new TextDecryptBuffer(encryptedCombinedBuffer);
            var parsedEncryptedBuffer = textDecryptBuffer.ParseBuffer(password);

            if (ErrorHandler.IsErrorOccurred) return ErrorHandler.ErrorLog;

            InitBuffer = textDecryptBuffer.DecryptedInitBuffer;
            SaltBuffer = textDecryptBuffer.DecryptedSaltBuffer;

            var decryptedBuffer = EncryptDecryptText(parsedEncryptedBuffer, password, false);

            return Encoding.UTF8.GetString(decryptedBuffer, 0, decryptedBuffer.Length);
        }

        private byte[] EncryptDecryptText(byte[] textBuffer, string password, bool isEncrypt)
        {
            var passwordBuffer = GetPasswordBytes(password);

            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Mode = CipherMode.CBC;

                var cryptoTransform = CreateCryptoTransform(rijndael, passwordBuffer, isEncrypt);

                if (isEncrypt)
                {
                    return EncryptBuffer(cryptoTransform, textBuffer);
                }

                return DecryptBuffer(cryptoTransform, textBuffer);
            }
        }

        private byte[] EncryptBuffer(ICryptoTransform cryptoTransform, byte[] textBuffer)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    WriteCryptoStream(cryptoStream, textBuffer);
                    var encryptedBuffer = memoryStream.ToArray();

                    return encryptedBuffer;
                }
            }
        }

        private byte[] DecryptBuffer(ICryptoTransform cryptoTransform, byte[] textBuffer)
        {
            byte[] decryptBuffer = new byte[textBuffer.Length];
            using (var memoryStream = new MemoryStream(textBuffer))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    int readLength = ReadCryptoStream(cryptoStream, decryptBuffer);

                    var decryptedBuffer = new byte[readLength];
                    Buffer.BlockCopy(decryptBuffer, 0, decryptedBuffer, 0, readLength);

                    return decryptedBuffer;
                }
            }
        }

        private void WriteCryptoStream(CryptoStream cryptoStream, byte[] textBuffer)
        {
            cryptoStream.Write(textBuffer, 0, textBuffer.Length);
            cryptoStream.FlushFinalBlock();
        }

        private int ReadCryptoStream(CryptoStream cryptoStream, byte[] decryptBuffer)
        {
            return cryptoStream.Read(decryptBuffer, 0, decryptBuffer.Length);
        }

        private ICryptoTransform CreateCryptoTransform(RijndaelManaged rijndael, byte[] passwordBuffer, bool isEnCrypted)
        {
            ICryptoTransform cryptoTransform;

            if (isEnCrypted)
            {
                cryptoTransform = rijndael.CreateEncryptor(passwordBuffer, InitBuffer);
            }
            else
            {
                cryptoTransform = rijndael.CreateDecryptor(passwordBuffer, InitBuffer);
            }

            return cryptoTransform;
        }

        private byte[] GetPasswordBytes(string password)
        {
            var passwordDeriveBytes = new PasswordDeriveBytes(password, SaltBuffer, GetHashAlgorithm(), 17);
            return passwordDeriveBytes.GetBytes(Constants.KeySize / 8);
        }

        private byte[] CreateRandomByteBuffer()
        {
            //Generates a cryptographic random number
            RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
            var buffer = new byte[Constants.InitSaltLength];
            cryptoServiceProvider.GetBytes(buffer);

            return buffer;
        }

        private string GetHashAlgorithm()
        {
            if (HashAlgorithm == HashAlgorithm.SHA256) return "SHA256";
            if (HashAlgorithm == HashAlgorithm.MD5) return "MD5";

            throw new ArgumentException("Invalid hash algorithm.");
        }
    }
}