using System;
using EncryptDecryptService;
using EncryptDecryptService.Utils;

namespace gitPlayground
{
    class Program
    {
        private static Cipher cipher = new Cipher();

        static void Main(string[] args)
        {
            var arguments = ArgumentParser.Parse(args);

            if (arguments != null && arguments.IsEncrypt) Encrypt(arguments);
        }

        private static string Encrypt(Arguments args)
        {
            return cipher.EncryptText(args.InputText, args.Password);
        }
    }
}
