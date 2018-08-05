﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutocalibrationRecorder : MonoBehaviour {
	
	public static float DriftPerSecond{
		get{
			if(PlayerPrefs.HasKey("autocorrectdrift")){
				return PlayerPrefs.GetFloat("autocorrectdrift");
			}else return 0;
		}
		private set{
			PlayerPrefs.SetFloat("autocorrectdrift", value);
		}
	}
	
	Transform t;
	
	void Start(){
		t = transform;
	}
	
	public void OnReceiveCommand(string data){
		if(data == "on"){//start calibration
			StartCoroutine(calibration());
		}else if(data == "off"){//stop calibration
			stopCalibration = true;
		}else if(data == "reset"){
			DriftPerSecond = 0;
		}
	}
	
	bool stopCalibration;
	
	IEnumerator calibration(){
		stopCalibration = false;
		
		Vector3 previousAngles;
		Vector3 angles = t.eulerAngles;
		float elapsed = 0;
		float yawDrift = 0;//this will keep track of how much the gearVR rotates
		
		while(!stopCalibration){
			yield return null;
			elapsed += Time.deltaTime;
			
			previousAngles = angles;
			angles = t.eulerAngles;
			
			//get difference in yaws (add the shortest way it couldve gone from previous angles to current angles)
			yawDrift += Mathf.DeltaAngle(previousAngles.y, angles.y);
			
		}
		
		//calibration over - compute how much we drifted per second
		DriftPerSecond = yawDrift / elapsed;
		print("Calibration result: " + DriftPerSecond + " (drifted " + yawDrift + " in " + elapsed + " seconds)");
		
	}
	
}