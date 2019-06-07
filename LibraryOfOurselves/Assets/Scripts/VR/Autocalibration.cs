using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autocalibration : MonoBehaviour {

	static float __driftPerSecond = float.NegativeInfinity;
	static float DriftPerSecond {
		get {
			if(__driftPerSecond != float.NegativeInfinity)
				return __driftPerSecond;
			__driftPerSecond = 0;
			if(HazePrefs.HasKey("autocorrectdrift")) {
				__driftPerSecond = HazePrefs.GetFloat("autocorrectdrift");
			}
			return __driftPerSecond;
		}
		set {
			__driftPerSecond = value;
			if(value == 0)
				HazePrefs.DeleteKey("autocorrectdrift");
			else
				HazePrefs.SetFloat("autocorrectdrift", value);
		}
	}

	public static Autocalibration Instance { get; private set; }

	bool calibrating = false;

	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}

	// status:
	//			0: Start recording calibration
	//			1: Stop recording calibration
	//			2: Reset to 0/second
	public void OnReceiveAutocalibrate(byte status) {
		switch(status) {
			case 0:
				if(!calibrating)
					StartCoroutine(calibration());
				break;
			case 1:
				calibrating = false;
				break;
			case 2:
				StopAllCoroutines();
				DriftPerSecond = 0;
				VRAdapter.Instance.SendAutocalibrationResult(1, 0);//1-> Finished (with result)
				break;
		}
	}

	//called when we get paired to a guide
	public void OnPair() {
		VRAdapter.Instance.SendAutocalibrationResult(1, DriftPerSecond);//1-> Finished (with result)
	}

	IEnumerator calibration() {
		calibrating = true;

		VRAdapter adapter = VRAdapter.Instance;
		adapter.SendAutocalibrationResult(0, 0);//0-> Started

		Transform cam = Camera.main.transform;
		Vector3 previousAngles;
		Vector3 angles = cam.eulerAngles;
		float elapsed = 0;
		float yawDrift = 0;//keep track of how much rotation happened

		while(calibrating) {
			yield return null;
			elapsed += Time.deltaTime;

			previousAngles = angles;
			angles = cam.eulerAngles;

			//get difference in yaw since last frame
			yawDrift += Mathf.DeltaAngle(previousAngles.y, angles.y);

			if(elapsed >= 5*60) {//stop automatically after 5 minutes
				calibrating = false;
			}
		}

		//Done - compute how much drift per second happened
		DriftPerSecond = yawDrift / elapsed;

		adapter.SendAutocalibrationResult(1, DriftPerSecond);//1-> Finished (with result)
	}

	private void Update() {
		if(DriftPerSecond != 0) {
			transform.Rotate(0, -DriftPerSecond * Time.deltaTime, 0);
		}
	}

}
