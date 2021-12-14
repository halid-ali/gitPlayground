namespace EncryptDecryptService.Utils
{
    internal static class ArgumentParser
    {
        internal static Arguments Parse(string[] args)
        {
            var inputText = ParseArgument(args, "-i", "--input");
            var password = ParseArgument(args, "-pw", "--password");
            var isEncrypt = IsEncrypt(args[0]);

            if (string.IsNullOrEmpty(inputText) || string.IsNullOrEmpty(password))
                return null;

            return new Arguments(inputText, password, isEncrypt);
        }

        private static string ParseArgument(string[] args, string shortName, string longName)
        {
            var text = string.Empty;

            foreach (var arg in args)
            {
                var argSplit = arg.Split('=');
                if (argSplit[0] == shortName || argSplit[0] == longName)
                {
                    text = argSplit[1];
                }
            }

            return text;
        }

        private static bool IsEncrypt(string argument)
        {
            return argument == "-e" || argument == "--encrypt";
        }
    }
}