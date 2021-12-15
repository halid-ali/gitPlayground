using System.Text;

namespace EncryptDecryptService.Utils
{
    internal static class ErrorHandler
    {
        private static StringBuilder _stringBuilder = new StringBuilder();

        internal static bool IsErrorOccurred { get; private set; } = false;

        internal static string ErrorLog => _stringBuilder.ToString();

        internal static void WrongPassword()
        {
            IsErrorOccurred = true;
            _stringBuilder.AppendLine("Error: Password is not correct!");
        }
    }
}