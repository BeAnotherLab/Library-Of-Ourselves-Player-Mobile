using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;

public class ListFiles : MonoBehaviour {
	
	[SerializeField] string extension;
	[SerializeField] StringEvent onFileFound;
	[SerializeField] bool verbose;

	private void OnEnable()
	{
		FileBrowser.RootFolderPicked += ListDataFolderFiles;
	}

	private void OnDisable()
	{
		FileBrowser.RootFolderPicked -= ListDataFolderFiles;
	}

	private void ListDataFolderFiles()
	{
		if (verbose) Haze.Logger.Log("Checking for " + extension + " files in " + DataFolder.GuidePath);
			
		foreach (FileSystemEntry file in FileBrowserHelpers.GetEntriesInDirectory(DataFolder.GuidePath, true))
		{
			Debug.Log("in this folder there is " + file.Path);
			if (file.Extension == extension)
			{
				onFileFound.Invoke(file.Path);
				if (verbose) Haze.Logger.Log("Found " + file.Path);
			}
		}
	}
	
}
