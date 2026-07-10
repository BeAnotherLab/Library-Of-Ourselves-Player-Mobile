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
        string filename = GetLocalPath(file.filename);

        if (!File.Exists(filename))
            return false;

        var info = new FileInfo(filename);
        if (info.Length != file.size)
            return false;

        return true; // (we’ll verify hash after download)
    }
}