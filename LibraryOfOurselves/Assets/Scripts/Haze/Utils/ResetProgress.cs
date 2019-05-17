using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetProgress : MonoBehaviour {
	
	[SerializeField] List<string> intsToKeep;
	[SerializeField] List<string> stringsToKeep;
	
	public void Reset(){
		Dictionary<string, int> ints = new Dictionary<string, int>();
		Dictionary<string, string> strings = new Dictionary<string, string>();
		
		foreach(string key in intsToKeep){
			if(HazePrefs.HasKey(key)){
				ints.Add(key, HazePrefs.GetInt(key));
			}
		}
		foreach(string key in stringsToKeep){
			if(HazePrefs.HasKey(key)){
				strings.Add(key, HazePrefs.GetString(key));
			}
		}
		
		HazePrefs.DeleteAll();
		
		foreach(KeyValuePair<string, int> pair in ints){
			HazePrefs.SetInt(pair.Key, pair.Value);
		}
		foreach(KeyValuePair<string, string> pair in strings){
			HazePrefs.SetString(pair.Key, pair.Value);
		}
		
	}
	
}
