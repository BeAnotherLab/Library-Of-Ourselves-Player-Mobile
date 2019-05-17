using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTapAnywhere : MonoBehaviour {
	
	[SerializeField] float minTimeAfterSceneLoad = 0;
	[SerializeField] UnityEvent onTap;
	[SerializeField] bool verbose = false;
	
	bool tapped = false;
	
	void Update(){
		if(tapped) return;
		
		if(Time.timeSinceLevelLoad >= minTimeAfterSceneLoad && Input.GetMouseButton(0)){
			tapped = true;
			if(verbose) print("Tapped the screen at "+Time.timeSinceLevelLoad+".");
			onTap.Invoke();
		}
		
	}
	
}
