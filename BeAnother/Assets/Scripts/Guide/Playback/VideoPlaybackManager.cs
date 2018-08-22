using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VideoPlaybackManager : MonoBehaviour {
	
	[SerializeField] UnityEvent DoPlay;
	[SerializeField] UnityEvent DoPause;
	[SerializeField] UnityEvent DoStop;
	[SerializeField] UnityEvent OnHang;
	[SerializeField] UnityEvent OnResume;
	
	bool playing = false;
	bool stopped = true;
	bool hanged = false;
	
	public void Play(){
		playing = true;
		stopped = false;
		if(!hanged)
			doPlay();
	}
	
	void doPlay(){
		DoPlay.Invoke();
	}
	
	public void Pause(){
		playing = false;
		stopped = false;
		if(!hanged)
			doPause();
	}
	
	void doPause(){
		DoPause.Invoke();
	}
	
	public void Stop(){
		playing = false;
		stopped = true;
		DoStop.Invoke();
	}
	
	public void Hang(){
		hanged = true;
		if(!stopped){
			doPause();//pause but without updating playing and stopped vars
		}
	}
	
	public void Resume(){
		hanged = false;
		if(!stopped && playing){
			doPlay();//resume playing if it was already
		}
	}
	
}
