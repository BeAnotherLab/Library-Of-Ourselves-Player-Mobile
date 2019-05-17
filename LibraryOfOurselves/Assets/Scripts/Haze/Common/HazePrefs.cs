/*
	This script is a wrapper around standard Unity PlayerPrefs.
	Wrapping PlayerPrefs like this makes it easy to change serialization methods,
	especially for different platforms, by editing only this one file.
	All Haze assets will use HazePrefs. Feel free to integrate it as well in your
	own projects for better control down the line! :)
	
	You can also use this file to make Haze assets use your preferred preferences
	system.
	
	As it is, I'm providing this file as a helper for you. You can modify it, and
	unlike other files from Haze assets, you're free to pass this one along.
	
	Keep this header if you want to~
	
	Jonathan Kings from Haze
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazePrefs {
	
	/** Deletes all key-value pairs */
	public static void DeleteAll(){
		PlayerPrefs.DeleteAll();
	}
	
	/** Deletes one key-value pair */
	public static void DeleteKey(string key){
		PlayerPrefs.DeleteKey(key);
	}
	
	/** Gets a float value from its key */
	public static float GetFloat(string key){
		return PlayerPrefs.GetFloat(key);
	}
	
	/** Gets an int value from its key */
	public static int GetInt(string key){
		return PlayerPrefs.GetInt(key);
	}
	
	/** Gets a string value from its key */
	public static string GetString(string key){
		return PlayerPrefs.GetString(key);
	}
	
	/** Returns true if the current device has a value for the preference key */
	public static bool HasKey(string key){
		return PlayerPrefs.HasKey(key);
	}
	
	/** Saves preferences */
	public static void Save(){
		PlayerPrefs.Save();
	}
	
	/** Sets a float value for a given key; should create the key-value pair if it doesn't exist. */
	public static void SetFloat(string key, float val){
		PlayerPrefs.SetFloat(key, val);
	}
	
	/** Sets an int value for a given key; should create the key-value pair if it doesn't exist. */
	public static void SetInt(string key, int val){
		PlayerPrefs.SetInt(key, val);
	}
	
	/** Sets a string value for a given key; should create the key-value pair if it doesn't exist. */
	public static void SetString(string key, string val){
		PlayerPrefs.SetString(key, val);
	}
	
}
