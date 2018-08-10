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
			
			sdroot = getSDCardPath();
			//remove any appended "Android/data/sco.forgotten.beanother/files"
			sdroot = sdroot.Split(new string[]{"/Android/data"}, StringSplitOptions.None)[0];
			sdroot += "/";
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
		using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
			using(AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")){
				AndroidJavaObject[] externalFilesDirs = context.Call<AndroidJavaObject[]>("getExternalFilesDirs", null);
				AndroidJavaObject emulated = null, sd = null;
				
				for(int i = 0; i<externalFilesDirs.Length; ++i){
					AndroidJavaObject dir = externalFilesDirs[i];
					using(AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment")){
						if(environment.CallStatic<bool>("isExternalStorageEmulated", dir))
							emulated = dir;
						else if(environment.CallStatic<bool>("isExternalStorageRemovable", dir))
							sd = dir;
					}
				}
				
				//return sd card in priority
				if(sd != null)
					return sd.Call<string>("getAbsolutePath");
				else if(emulated != null)
					return sd.Call<string>("getAbsolutePath");
				else
					return Application.persistentDataPath;
			}
		}
		#else
		return Application.persistentDataPath;
		#endif
	}
	
}
