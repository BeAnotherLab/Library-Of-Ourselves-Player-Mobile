using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BinauralAudio : MonoBehaviour {
	
	static string LEFT = "-l";
	static string RIGHT = "-r";
	static string EXTENSION = ".mp3";

	[SerializeField] AudioSource leftSource;
	[SerializeField] AudioSource rightSource;
	
	public static bool Exists(string name){
		return File.Exists(name + LEFT + EXTENSION) && File.Exists(name + RIGHT + EXTENSION);
	}
	
	public void Load(string name){
		WWW www = new WWW("file://"+name+LEFT+EXTENSION);
		AudioClip left = www.GetAudioClip();
		leftSource.clip = left;
		
		www = new WWW("file://"+name+RIGHT+EXTENSION);
		AudioClip right = www.GetAudioClip();
		rightSource.clip = right;
	}
	
	public void Play(){
		leftSource.Play();
		rightSource.Play();
	}
	
	public void Pause(){
		leftSource.Pause();
		rightSource.Pause();
	}
	
	public void Stop(){
		leftSource.Stop();
		rightSource.Stop();
	}
	
}
