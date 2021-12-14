namespace EncryptDecryptService.Buffers
{
    internal sealed class TextDecryptBuffer : BaseDecryptBuffer
    {
        public TextDecryptBuffer(byte[] combinedBuffer) : base(combinedBuffer)
        { }

        internal byte[] ParseBuffer(string password)
        {
            return ParseCombinedBuffer(password);
        }
    }
}