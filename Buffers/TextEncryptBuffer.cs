namespace EncryptDecryptService.Buffers
{
    internal sealed class TextEncryptBuffer : BaseEncryptBuffer
    {
        public TextEncryptBuffer(string password, byte[] initBuffer, byte[] saltBuffer, byte[] encryptedBuffer) :
            base(password, initBuffer, saltBuffer, encryptedBuffer)
        { }

        internal byte[] CombineBuffer()
        {
            PreconditionCheck();

            var combinedBufferLength = 0;
            combinedBufferLength += Constants.Int32Length;
            combinedBufferLength += PasswordBuffer.Length;
            combinedBufferLength += InitBuffer.Length;
            combinedBufferLength += SaltBuffer.Length;
            combinedBufferLength += Constants.Int32Length;
            combinedBufferLength += EncryptedBuffer.Length;

            var position = 0;
            CombinedBuffer = new byte[combinedBufferLength];

            CreateCombinedBuffer(position);

            return CombinedBuffer;
        }
    }
}