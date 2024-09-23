using System;
using System.Security.Cryptography;
using System.Text;

public class AesGcmEncryption
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("ThisIsASecretKeyThisIsASecretKey"); // 32 bytes for AES-256

    public static byte[] Encrypt(string plainText)
    {
        using (AesGcm aesGcm = new AesGcm(Key, AesGcm.TagByteSizes.MaxSize))
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            RandomNumberGenerator.Fill(nonce);

            byte[] cipherText = new byte[plainTextBytes.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

            aesGcm.Encrypt(nonce, plainTextBytes, cipherText, tag);

            byte[] encryptedMessage = new byte[nonce.Length + cipherText.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, encryptedMessage, 0, nonce.Length);
            Buffer.BlockCopy(cipherText, 0, encryptedMessage, nonce.Length, cipherText.Length);
            Buffer.BlockCopy(tag, 0, encryptedMessage, nonce.Length + cipherText.Length, tag.Length);

            return encryptedMessage;
        }
    }

    public static string Decrypt(byte[] encryptedMessage)
    {
        using (AesGcm aesGcm = new AesGcm(Key, AesGcm.TagByteSizes.MaxSize))
        {
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
            byte[] cipherText = new byte[encryptedMessage.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(encryptedMessage, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(encryptedMessage, nonce.Length, cipherText, 0, cipherText.Length);
            Buffer.BlockCopy(encryptedMessage, nonce.Length + cipherText.Length, tag, 0, tag.Length);

            byte[] plainText = new byte[cipherText.Length];
            aesGcm.Decrypt(nonce, cipherText, tag, plainText);

            return Encoding.UTF8.GetString(plainText);
        }
    }
}
