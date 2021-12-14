using System;

public class Cipher
{
    public Cipher() : this(HashAlgorithm.SHA256)
    { }

    public Cipher(HashAlgorithm hashAlgorithm)
    {
        HashAlgorithm = hashAlgorithm;
    }

    public HashAlgorithm HashAlgorithm { get; }

    public string EncryptText(string plainText, string password)
    {
        throw new NotImplementedException();
    }

    public string DecryptText(string encryptedText, string password)
    {
        throw new NotImplementedException();
    }
}