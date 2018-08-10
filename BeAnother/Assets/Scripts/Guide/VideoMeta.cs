using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VideoMeta {
	
	public string description = "";//300 characters description
	public string objects = "";//a few objects that the guide will need
	public float pitch = 0;
	public float yaw = 0;
	public float roll = 0;
	
	static string pathFromName(string videoName){
		//return Filesystem.SDCardRoot + videoName + ".json";
		return Filesystem.PersistentDataPath + videoName + ".json";
	}
	
	public static bool HasMeta(string videoName){
		return File.Exists(pathFromName(videoName));
	}
	
	public static VideoMeta LoadMeta(string videoName){
		string path = pathFromName(videoName);
		if(File.Exists(path)){
			string data = Filesystem.ReadFile(path);
			VideoMeta vm = JsonUtility.FromJson<VideoMeta>(data);
			return vm;
		}else{
			VideoMeta vm = new VideoMeta();
			//SaveMeta(videoName, vm);
			return vm;//don't have that yet :3
		}
	}
	
	public static void SaveMeta(string videoName, VideoMeta vm){
		string data = JsonUtility.ToJson(vm);
		string path = pathFromName(videoName);
		//StreamWriter sw = new StreamWriter(path, false);
		//sw.WriteLine(data);
		//sw.Close();
		if(!Filesystem.WriteFile(path, data))
			Debug.LogError(Filesystem.LastException);
	}
	
}
