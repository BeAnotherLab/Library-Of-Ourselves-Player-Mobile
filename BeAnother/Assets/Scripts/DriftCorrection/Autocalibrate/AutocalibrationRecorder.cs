using System.Collections;
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
	
	void say(string s){
		print(s);
		Sender sender = GetComponent<Sender>();
		if(sender != null)
			sender.Send(s);
	}
	
	public void OnReceiveCommand(string data){
		if(data == "on"){//start calibration
			StartCoroutine(calibration());
			say("Calibrating...");
		}else if(data == "off"){//stop calibration
			stopCalibration = true;
		}else if(data == "reset"){
			StopAllCoroutines();
			DriftPerSecond = 0;
			say("Calibration reset to 0.");
		}else if(data == "fetch"){
			if(DriftPerSecond == 0)
				say("No drift correction.");
			else
				say("Calibration result: drifted " + DriftPerSecond + " degrees per second.");
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
		
		say("Calibration result: drifted " + DriftPerSecond + " degrees per second ("+ yawDrift + " over " + elapsed + " seconds).");
	}
	
}
