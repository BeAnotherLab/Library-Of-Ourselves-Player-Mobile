using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class HashUtil
{
    public static string Sha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);

        var hash = sha.ComputeHash(stream);
        return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}