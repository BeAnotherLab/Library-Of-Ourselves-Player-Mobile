using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//place this on the parent of the vr camera
public class VRCameraLock : MonoBehaviour {
	
	[SerializeField] bool lockRotation = false;
	[SerializeField] new Transform camera;
	
	Transform t;
	
	void Start(){
		t = transform;
	}
	
	void LateUpdate(){
		if(lockRotation){
			t.eulerAngles = Vector3.zero;
			t.Rotate(-camera.eulerAngles);
		}
	}
	
}
