using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VideoMeta {
	
	public string description = "";//300 characters description
	public string[] objects = new string[]{};//a few objects that the guide will need
	public string imagePath = "";//relative path to an image to represent this video
	
	static string pathFromName(string videoName){
		return Filesystem.SDCardRoot + videoName + ".json";
	}
	
	public static VideoMeta LoadMeta(string videoName){
		string path = pathFromName(videoName);
		if(File.Exists(path)){
			string data = File.ReadAllText(path);
			VideoMeta vm = JsonUtility.FromJson<VideoMeta>(data);
			return vm;
		}else{
			return new VideoMeta();//don't have that yet :3
		}
	}
	
	public static void SaveMeta(string videoName, VideoMeta vm){
		string data = JsonUtility.ToJson(vm);
		string path = pathFromName(videoName);
		StreamWriter sw = new StreamWriter(path, false);
		sw.WriteLine(data);
		sw.Close();
	}
	
}
