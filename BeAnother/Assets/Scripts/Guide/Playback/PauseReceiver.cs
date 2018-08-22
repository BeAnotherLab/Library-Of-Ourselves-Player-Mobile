using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseReceiver : MonoBehaviour {
	
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onResume;
	
	public void OnReceive(string data){
		if(data == "pause"){
			onPause.Invoke();
		}else if(data == "resume"){
			onResume.Invoke();
		}else{
			Debug.LogError("Received " + data + ", should be either pause or resume...");
		}
	}
	
}
