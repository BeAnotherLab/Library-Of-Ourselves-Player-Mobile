using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotations : MonoBehaviour {
	
	[SerializeField] Transform cam;//the main camera, to be able to calibrate
	[SerializeField] OVRInput.Button oculusGoRecenterButton = OVRInput.Button.PrimaryIndexTrigger;//change this to change which button controls recenter on oculus go
	[SerializeField] GvrControllerButton mirageRecenterButton = GvrControllerButton.TouchPadButton;//change this to change which button controls recenter on mirage solo
	
	float firstTap = 0;
	
	public void Calibrate(){
		//match this gameobject's rotation with the camera's
		//only correct yaw:
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y, transform.eulerAngles.z);
	}
	
	public void Correct(float pitch, float yaw, float roll){
		//all corrections happen in our child ("Rotation Corrections")
		transform.GetChild(0).localEulerAngles = new Vector3(pitch, yaw, roll);
	}
	
	void Update(){
		if(VRDevice.OculusGo){//On OculusGo, use the controller's trigger to recalibrate
			OVRInput.Update();
			if(OVRInput.Get(oculusGoRecenterButton)){
				Calibrate();
			}
		}else if(VRDevice.MirageSolo){//On Mirage Solo, use the controller's click button
			if(GvrControllerInput.GetDevice(GvrControllerHand.Dominant).GetButton(mirageRecenterButton)){
				Calibrate();
			}
		}else if(VRDevice.GearVR){//On GearVR, double-tap to recalibrate
			if(firstTap > 0){//we've tapped a first time!
				if(Input.GetMouseButtonDown(0)){
					//that's it, double-tapped.
					Calibrate();
					firstTap = 0;
				}
				firstTap -= Time.deltaTime;
			}else if(Input.GetMouseButtonUp(0)){
				firstTap = 0.3f;//you have .3 seconds to tap once more for double tap!
			}
		}
	}
	
	void FixedUpdate(){
		if(VRDevice.OculusGo)
			OVRInput.FixedUpdate();
	}
	
}
