using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EncryptDecryptService.Buffers;

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

        public string DecryptText(string encryptedText, string password)
        {
            throw new NotImplementedException();
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

                //TODO: implement DecryptBuffer() method and call it from here
                throw new NotImplementedException();
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

        private void WriteCryptoStream(CryptoStream cryptoStream, byte[] textBuffer)
        {
            cryptoStream.Write(textBuffer, 0, textBuffer.Length);
            cryptoStream.FlushFinalBlock();
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