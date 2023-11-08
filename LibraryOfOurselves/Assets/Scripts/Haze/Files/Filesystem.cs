using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Filesystem {
	
	static Exception latestException = null;
	public static Exception LastException{
		get{ return latestException; }
	}
	
	static string sdroot = "";
	public static string SDCardRoot{
		get{
			if(sdroot != "") return sdroot;

#if UNITY_ANDROID && !UNITY_EDITOR
			sdroot = getSDCardPath();
			//remove any appended "Android/data/sco.haze.xxxx/files"
			sdroot = sdroot.Split(new string[]{"/Android/data/"}, StringSplitOptions.None)[0];
			sdroot += "/";
#elif UNITY_EDITOR
			sdroot = Application.dataPath.Split(new string[] { Application.productName.Replace(" ", "") + "/Assets" }, StringSplitOptions.None)[0];
#else
			sdroot = Application.dataPath.Split(new string[] { Application.productName + "_Data" }, StringSplitOptions.None)[0];
#endif
			return sdroot;
		}
	}
	
	public static string PersistentDataPath{
		get{
			return Application.persistentDataPath + "/";
		}
	}
	
	public static bool WriteFile(string path, string contents){
		try{
			File.WriteAllText(path, contents);
			return true;
		}catch(Exception e){
			latestException = e;
			return false;
		}
	}
	
	public static string ReadFile(string path){
		try{
			return File.ReadAllText(path);
		}catch(Exception e){
			latestException = e;
			return null;
		}
	}
	
	static string getSDCardPath(){
#if UNITY_ANDROID && !UNITY_EDITOR
		//string path = VrPlayerBindings.Instance.GetSDCardPath();
		//if(path != null) return path;
		return Application.persistentDataPath;
#else
		return Application.persistentDataPath;
#endif
	}
	
}
