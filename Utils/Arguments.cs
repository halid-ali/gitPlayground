namespace EncryptDecryptService.Utils
{
    internal class Arguments
    {
        public Arguments(string inputText, string password, bool isEncrypt)
        {
            InputText = inputText;
            Password = password;
            IsEncrypt = isEncrypt;
        }

        public string InputText { get; set; }

        public string Password { get; set; }

        public bool IsEncrypt { get; set; }

        public override string ToString()
        {
            return $"Input:{InputText}\nPassword:{Password}\nIsEncrypt:{IsEncrypt}";
        }
    }
}