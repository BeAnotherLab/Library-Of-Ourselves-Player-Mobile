using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Lang : MonoBehaviour {
	
	public class Language{
		string hello = "Hello";
	}
	
	static Lang instance = null;
	
	[SerializeField] string language = "en"; 
	
	Language current;
	
	//usage, anywhere in the code: string hello = Lang.Uage.hello;
	static Language Uage{
		get{
			return instance.current;
		}
	}
	
	void Start(){
		if(instance != null) return;
		DontDestroyOnLoad(this);
		instance = this;
		instance.current = getLang(language);
	}
	
	static string pathFromName(string l){
		return Filesystem.SDCardRoot + l + ".json";
	}
	
	//load from .json
	static Language getLang(string l){
		string path = pathFromName(l);
		if(File.Exists(path)){
			print(path + " read.");
			string data = File.ReadAllText(path);
			Language lan = JsonUtility.FromJson<Language>(data);
			return lan;
		}else{
			//we dont have that language yet, create an empty json file for it that should then be filled manually outside the app
			Language lan = new Language();
			string data = JsonUtility.ToJson(lan);
			print("Writing json to " + path);
			StreamWriter sw = new StreamWriter(path, false);
			sw.WriteLine(data);
			sw.Close();
			return lan;//for now, use this
		}
	}
	
}