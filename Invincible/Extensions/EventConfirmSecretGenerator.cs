using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;

namespace Invincible.Extensions;

public static class EventConfirmSecretGenerator
{
    private static byte[] IV =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };

    public static Task<byte[]> getSecret(ObjectId object_id) => EncryptAsync(object_id.ToString(), "Ukrainians are Invincible");
    public static async Task<ObjectId> readSecret(byte[] secret)
    {
        var decrypted_message = await DecryptAsync(secret, "Ukrainians are Invincible");
        return new ObjectId(decrypted_message);
    }

    private static byte[] generateSecret(string passphrase)
    {
        var empty_salt = Array.Empty<byte>();
        var iterations = 1000;
        var desired_key_length = 16; // 16 bytes equal 128 bits.
        var hash_method = HashAlgorithmName.SHA384;
        return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(passphrase),
                                         empty_salt,
                                         iterations,
                                         hash_method,
                                         desired_key_length);
    }

    private static async Task<byte[]> EncryptAsync(string clearText, string passphrase)
    {
        using Aes aes = Aes.Create();
        aes.Key = generateSecret(passphrase);
        aes.IV = IV;
        using MemoryStream output = new();
        using CryptoStream crypto_stream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await crypto_stream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
        await crypto_stream.FlushFinalBlockAsync();
        return output.ToArray();
    }

    private static async Task<string> DecryptAsync(byte[] encrypted, string passphrase)
    {
        using Aes aes = Aes.Create();
        aes.Key = generateSecret(passphrase);
        aes.IV = IV;
        using MemoryStream input = new(encrypted);
        using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream output = new();
        await cryptoStream.CopyToAsync(output);
        return Encoding.Unicode.GetString(output.ToArray());
    }
}
