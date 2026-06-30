using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DeletePersistentData : MonoBehaviour
{
    public static void ClearPersistentData()
    {
        string path = Application.persistentDataPath;

        if (!Directory.Exists(path))
            return;

        // Delete all files
        foreach (string file in Directory.GetFiles(path))
        {
            File.Delete(file);
        }

        // Delete all subdirectories
        foreach (string dir in Directory.GetDirectories(path))
        {
            Directory.Delete(dir, true); // true = recursive
        }

        Debug.Log($"Cleared persistent data at: {path}");
    }
}
