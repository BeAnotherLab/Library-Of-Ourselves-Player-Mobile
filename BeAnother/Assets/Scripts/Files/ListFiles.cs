using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ListFiles : MonoBehaviour {
	
	[SerializeField] string extension = "txt";
	[SerializeField] StringEvent onFileFound;
	[SerializeField] bool verbose = false;
	
	void Start(){
		string dir = Filesystem.SDCardRoot;
		if(verbose)
			print("Checking for " + extension + " files in " + dir);
		foreach(string file in Directory.GetFiles(dir)){
			//check if extension is <extension>
			string[] parts = file.Split('.');
			string ext = parts[parts.Length-1];
			if(ext == extension){
				onFileFound.Invoke(file);
				if(verbose) print("Found " + file);
			}
		}
	}
	
}
