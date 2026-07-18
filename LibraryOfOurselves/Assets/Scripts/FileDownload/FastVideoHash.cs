using System;
using System.IO;
using System.Security.Cryptography;

public static class FastVideoHash
{
    // 10MB is plenty to capture the entire video index/header structure
    private const int ChunkSize = 10 * 1024 * 1024; 

    public static string ComputeHeadTailSha256(string filePath)
    {
        if (!File.Exists(filePath)) return string.Empty;

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            long fileLength = fs.Length;
            byte[] combinedSample;

            // If the file is smaller than our target boundary size, just hash the whole thing
            if (fileLength <= ChunkSize * 2)
            {
                combinedSample = new byte[fileLength];
                fs.Read(combinedSample, 0, (int)fileLength);
            }
            else
            {
                combinedSample = new byte[ChunkSize * 2];

                // Read the first 10MB (Head)
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(combinedSample, 0, ChunkSize);

                // Read the last 10MB (Tail)
                fs.Seek(-ChunkSize, SeekOrigin.End);
                fs.Read(combinedSample, ChunkSize, ChunkSize);
            }

            // Hash only this combined 20MB array instead of 10GB
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedSample);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}