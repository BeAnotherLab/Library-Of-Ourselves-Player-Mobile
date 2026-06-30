using System.IO;
using UnityEngine;

public static class FileUtil
{
    public static string GetLocalPath(string relativePath)
    {
        return Path.Combine(Application.persistentDataPath, relativePath);
    }

    public static bool ExistsAndMatches(ManifestFile file)
    {
        string path = GetLocalPath(file.path);

        if (!File.Exists(path))
            return false;

        var info = new FileInfo(path);
        if (info.Length != file.size)
            return false;

        return true; // (we’ll verify hash after download)
    }
}