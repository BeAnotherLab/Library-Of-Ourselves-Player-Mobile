using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotations : MonoBehaviour {
	
	[SerializeField] Transform cam;//the main camera, to be able to calibrate
	
	float firstTap = 0;
	
	public void Calibrate(){
		//match this gameobject's rotation with the camera's
		transform.rotation = cam.rotation;
	}
	
	public void Correct(float pitch, float yaw, float roll){
		//all corrections happen in our child ("Rotation Corrections")
		transform.GetChild(0).localEulerAngles = new Vector3(pitch, yaw, roll);
	}
	
	void Update(){
		if(VRDevice.GearVR){//On GearVR, double-tap to recalibrate
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
		}else if(VRDevice.OculusGo){
			//something else?
			OVRInput.Update();
			if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)){
				Calibrate();
			}
		}
	}
	
	void FixedUpdate(){
		if(VRDevice.OculusGo)
			OVRInput.FixedUpdate();
	}
	
}
