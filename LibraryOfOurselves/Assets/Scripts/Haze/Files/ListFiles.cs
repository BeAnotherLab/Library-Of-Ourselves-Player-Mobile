using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ListFiles : MonoBehaviour {
	
	[SerializeField] string extension = "txt";
	[SerializeField] StringEvent onFileFound;
	[SerializeField] bool verbose;
	
	void Start()
	{
		if (verbose) Haze.Logger.Log("Checking for " + extension + " files in " + DataFolder.GuidePath);
			
		foreach (string file in Directory.GetFiles(DataFolder.GuidePath))
		{
			Debug.Log("in this folder there is " + file);
			//check if extension is <extension>
			string[] parts = file.Split('.');
			string ext = parts[parts.Length-1];
			if (ext == extension)
			{
				onFileFound.Invoke(file);
				if (verbose) Haze.Logger.Log("Found " + file);
			}
		}
	}
	
}
