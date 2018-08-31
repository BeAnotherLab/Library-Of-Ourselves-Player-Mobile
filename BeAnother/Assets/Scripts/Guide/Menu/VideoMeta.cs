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
	
	static string pathFromName(string videoName, bool sd = false){
		if(sd) return Filesystem.SDCardRoot + videoName + "Info.json";
		return Filesystem.PersistentDataPath + videoName + "Info.json";
	}
	
	public static bool HasMeta(string videoName){
		return File.Exists(pathFromName(videoName, true) || File.Exists(pathFromName(videoName));
	}
	
	public static VideoMeta LoadMeta(string videoName){
		string path = pathFromName(videoName, true);//try sd root
		if(!File.Exists(path))
			path = pathFromName(videoName);//fallback to regular directory
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
