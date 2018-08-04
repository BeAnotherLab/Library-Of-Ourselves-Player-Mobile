using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class DriftCorrection : MonoBehaviour {
	
	[SerializeField] ARCoreSession arCore;
	[SerializeField] int minTrackables = 5;
	[SerializeField] [Range(0.0f, 1.0f)] float influence = 0.1f;//how much ar core influences the yaw of the gearvr per second
	
	bool correctDrift = true;//this can be enabled or disabled at anytime. to completely & permanently disable drift correction, use DriftCorrection.enabled = false instead.
	float yawCorrection = 0;
	
	public bool CorrectDrift{
		set{
			correctDrift = value;
			//enable or disable vr lock
			GetComponent<VRCameraLock>().enabled = correctDrift;
			//enable or disable arcore
			arCore.gameObject.SetActive(correctDrift);
		}
	}
	
	/** True if currently ARCore is reporting good tracking */
	bool IsTrackingOk{
		get{
			if(Session.Status == SessionStatus.Tracking){
				//check how many trackables we have currently
				if(minTrackables <= 0) return true;//bypass this check
				List<Trackable> trackables = new List<Trackable>();
				Session.GetTrackables<Trackable>(trackables, TrackableQueryFilter.All);
				if(trackables.Count > minTrackables){
					return true;//we have enough features detected.
				}
			}
			return false;
		}
	}
	
	void LateUpdate(){
		
		Vector3 angles = transform.eulerAngles;
		float gearVrYaw = angles.y;
		float previousCorrectedYaw = gearVrYaw + yawCorrection;
		
		if(correctDrift && influence > 0 && IsTrackingOk){
			
			float arCoreYaw = arCore.transform.eulerAngles.y;
			//check difference between arCoreYaw and gearVrYaw
			float difference = Mathf.DeltaAngle(previousCorrectedYaw, arCoreYaw);
			yawCorrection += difference * influence * Time.deltaTime;//correct this value slightly
			
		}
		
		//apply correction for this frame
		angles.y = 0;
		transform.eulerAngles = angles;
		transform.Rotate(new Vector3(0, gearVrYaw + yawCorrection, 0), Space.World);
	}
	
}