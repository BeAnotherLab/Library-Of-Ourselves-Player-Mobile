using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundMute : MonoBehaviour {
	
	[SerializeField] UnityEvent onMute;
	[SerializeField] UnityEvent onUnmute;
	
	bool Muted{
		get{
			return AudioListener.volume == 0;
		}
		set{
			AudioListener.volume = value ? 0 : 1;
			HazePrefs.SetInt("haze-muted", value ? 1 : 0);
		}
	}
	
	void OnEnable(){
		if(HazePrefs.HasKey("haze-muted")){
			Muted = HazePrefs.GetInt("haze-muted") == 1;
		}
		react();
	}
	
	void react(){
		if(Muted){
			onMute.Invoke();
		}else{
			onUnmute.Invoke();
		}
	}
	
	public void Toggle(){
		Muted = !Muted;
		react();
	}
	
	public void Mute(){
		if(!Muted){
			Muted = true;
			react();
		}
	}
	
	public void Unmute(){
		if(Muted){
			Muted = false;
			react();
		}
	}
	
}
