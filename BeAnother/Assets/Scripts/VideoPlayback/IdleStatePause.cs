using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sender))]
public class IdleStatePause : MonoBehaviour {
	
	Sender sender;
	
	void Start(){
		sender = GetComponent<Sender>();
	}
	
	void OnPause(){
		sender.Send("pause");
	}
	
	void OnResume(){
		sender.Send("resume");
	}
	
	void OnApplicationPause(bool paused){
		if(paused){
			OnPause();
		}else{
			OnResume();
		}
	}
	
	void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus){
			OnPause();
		}else{
			OnResume();
		}
    }
	
}