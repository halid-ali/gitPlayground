using System;
using System.Text;

namespace EncryptDecryptService.Buffers
{
    internal abstract class BaseDecryptBuffer
    {
        public BaseDecryptBuffer(byte[] combinedBuffer)
        {
            CombinedBuffer = combinedBuffer;
        }

        public byte[] DecryptedInitBuffer { get; private set; }

        public byte[] DecryptedSaltBuffer { get; private set; }

        protected byte[] CombinedBuffer { get; }

        protected byte[] ParseCombinedBuffer(string password, int position = 0)
        {
            //read password length
            var passwordLengthBuffer = new byte[Constants.Int32Length];
            Buffer.BlockCopy(CombinedBuffer, position, passwordLengthBuffer, 0, passwordLengthBuffer.Length);
            position += passwordLengthBuffer.Length;

            //read password
            var passwordLength = BitConverter.ToInt32(passwordLengthBuffer, 0);
            var passwordBuffer = new byte[passwordLength];
            Buffer.BlockCopy(CombinedBuffer, position, passwordBuffer, 0, passwordBuffer.Length);
            position += passwordBuffer.Length;

            if (!CheckPassword(password, GetParsedPassword(passwordBuffer))) return new byte[0];

            //read init buffer
            DecryptedInitBuffer = new byte[Constants.InitSaltLength];
            Buffer.BlockCopy(CombinedBuffer, position, DecryptedInitBuffer, 0, DecryptedInitBuffer.Length);
            position += DecryptedInitBuffer.Length;

            //read salt buffer
            DecryptedSaltBuffer = new byte[Constants.InitSaltLength];
            Buffer.BlockCopy(CombinedBuffer, position, DecryptedSaltBuffer, 0, DecryptedSaltBuffer.Length);
            position += DecryptedSaltBuffer.Length;

            //read encrypted buffer length
            var encryptedLengthBuffer = new byte[Constants.Int32Length];
            Buffer.BlockCopy(CombinedBuffer, position, encryptedLengthBuffer, 0, encryptedLengthBuffer.Length);
            position += encryptedLengthBuffer.Length;

            //read encrypted data
            var encryptedLength = BitConverter.ToInt32(encryptedLengthBuffer, 0);
            var encryptedBuffer = new byte[encryptedLength];
            Buffer.BlockCopy(CombinedBuffer, position, encryptedBuffer, 0, encryptedBuffer.Length);

            return encryptedBuffer;
        }

        private bool CheckPassword(string actualPassword, string parsedPassword)
        {
            return actualPassword == parsedPassword;
        }

        private string GetParsedPassword(byte[] passwordBuffer)
        {
            return Encoding.UTF8.GetString(passwordBuffer, 0, passwordBuffer.Length);
        }
    }
}