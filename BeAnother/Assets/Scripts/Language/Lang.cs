using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;

public class Lang : MonoBehaviour {
	
	public class Language{
		public string searching = "Searching for user device...";
		public string userDisconnected = "User device disconnected. Waiting for reconnection...";
		public string title = "Library of [Ourselves]";
		public string back = "Back";
		public string choose = "Choose";
		public string start = "Start the VR film";
		public string play = "Play";
		public string pause = "Pause";
		public string stop = "Stop";
		public string recenter = "Recenter";
		public string fps = "FPS: ";
		public string battery = "Battery: ";
		public string pitch = "Pitch";
		public string yaw = "Yaw";
		public string roll = "Roll";
		public string save = "Save";
		public string reset = "Reset";
		public string cancel = "Cancel";
		public string hanged = "User app paused";
		public string chooseAVRFilm = "Choose a VR film from the list below to read its synopsis";
		public string advancedSettings = "Advanced Settings";
		public string enterAdvancedSettingsCode = "Please enter the code to access advanced settings.";
		public string typeCode = "Type code";
		public string youWillNeed = "To perform this experience, the guide will need the following objects: ";
		public string youWillPlay = "You are going to play the VR film ";
		public string instructions = "There must be One Guide, with the tablet, and One User with the VR headset. The guide must follow the instructions below:";
		public string instruction1 = "1. Place the following objects nearby: ";
		public string instruction2 = "2. Help the user to sit down and properly adjust the User's VR headset and headphones.";
		public string instruction3 = "3. Put the Guide's earplugs on.";
		public string instruction4 = "4. Follow all the instructions that will be given to you.";
		public string instruction5 = "5. IMPORTANT: take you role seriously, the safety of the user relies on you.";
		public string instruction6 = "6. Make sure the user is looking straight and press Start.";
		public string typeDescription = "Type description";
		public string typeListOfObjects = "Type list of objects";
		public string notSaved = "You haven't saved your changes. Do you want to save them?";
		public string yesSave = "Save";
		public string noBack = "Discard";
		public string paired = "Paired to user device. Both devices can only connect to each other now.";
		public string unpaired = "Unpaired from user device. Both devices will accept other devices now.";
	}
	
	static Lang instance = null;
	
	[SerializeField] string language = "en";
	
	Language current;
	
	//usage, anywhere in the code: string hello = Lang.Uage.hello;
	public static Language Uage{
		get{
			return instance.current;
		}
	}
	
	void Start(){
		if(instance != null){
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(this);
		instance = this;
		instance.current = getLang(language);
	}
	
	static string pathFromName(string l){
		//return Filesystem.SDCardRoot + l + ".json";
		return Filesystem.PersistentDataPath + l + ".json";
	}
	
	//load from .json
	static Language getLang(string l){
		string path = pathFromName(l);
		if(File.Exists(path)){
			print(path + " read.");
			string data = Filesystem.ReadFile(path);
			Language lan = JsonUtility.FromJson<Language>(data);
			return lan;
		}else{
			//we dont have that language yet, create an empty json file for it that should then be filled manually outside the app
			Language lan = new Language();
			string data = JsonUtility.ToJson(lan);
			print("Writing json to " + path);
			//StreamWriter sw = new StreamWriter(path, false);
			//sw.WriteLine(data);
			//sw.Close();
			if(!Filesystem.WriteFile(path, data))
				Debug.LogError(Filesystem.LastException);
			return lan;//for now, use this
		}
	}
	
	public static string GetText(string key){
		Type language = typeof(Language);
		FieldInfo field = language.GetField(key);
		Language currentLanguage = Uage;
		return (string)field.GetValue(currentLanguage);
	}
	
}