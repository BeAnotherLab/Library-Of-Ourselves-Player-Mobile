using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotationReceiver : MonoBehaviour {
	
	[SerializeField] bool invert = false;
	
	Transform t;
	
	void Start(){
		t = transform;
	}
	
	public void OnReceiveRotation(string data){
		string[] dat = data.Split(' ');
		if(dat.Length > 2){
			Vector3 angles = new Vector3(f(dat[0]), f(dat[1]), f(dat[2]));
			t.rotation = Quaternion.identity;
			t.Rotate(angles, Space.World);
		}else Debug.LogError("Rotation received \"" + data + "\" is not a valid vector3.");
	}
	
	float f(string s){
		return (float)Convert.ToDouble(s) * (invert? -1 : 1);
	}
	
}