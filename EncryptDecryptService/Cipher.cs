using System;
using System.Security.Cryptography;

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

        public string EncryptText(string plainText, string password)
        {
            throw new NotImplementedException();
        }

        public string DecryptText(string encryptedText, string password)
        {
            throw new NotImplementedException();
        }

        private byte[] CreateRandomByteBuffer()
        {
            //Generates a cryptographic random number
            RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
            var buffer = new byte[Constants.InitSaltLength];
            cryptoServiceProvider.GetBytes(buffer);

            return buffer;
        }
    }
}