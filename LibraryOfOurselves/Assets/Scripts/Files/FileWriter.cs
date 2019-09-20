using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FileWriter{
    
	public static void WriteFile(string directory, string filename, string contents) {

		if(directory[directory.Length - 1] != '/' && directory[directory.Length - 1] != '\\') {
			directory += '/';
		}
		directory += "LOO-files/";

		string fullPath = directory + filename;

		if(!Directory.Exists(directory)) {
			Debug.Log("Creating directory: " + directory);
			Directory.CreateDirectory(directory);
			if(Directory.Exists(directory)) {
				Debug.Log("Created directory.");
			} else {
				Debug.LogError("Error: Could not create directory: " + directory);
				return;
			}
		}

		if(!File.Exists(fullPath)) {
			Debug.Log("Creating file: " + filename + " (" + fullPath + ")");
			File.Create(fullPath).Close();
			if(File.Exists(fullPath)) {
				Debug.Log("Created file.");
			} else {
				Debug.LogError("Error: Could not create file: " + filename + " (" + fullPath + ")");
			}
		}

		File.WriteAllText(fullPath, contents);

		Debug.Log("Wrote to " + fullPath);
	}

}
