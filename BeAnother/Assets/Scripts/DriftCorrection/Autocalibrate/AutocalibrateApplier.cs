using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutocalibrateApplier : MonoBehaviour {
	
	Transform t;
	bool antiDrift = false;
	
	void Start(){
		t = transform;
	}
	
	public void Recenter(){
		t.localEulerAngles = Vector3.zero;
	}
	
	public void Play(){
		antiDrift = true;
	}
	
	public void Pause(){
		antiDrift = false;
	}
	
	void Update(){
		if(antiDrift){
			//apply drift correction:
			t.Rotate(0, -AutocalibrationRecorder.DriftPerSecond * Time.deltaTime, 0);
		}
	}
	
}
