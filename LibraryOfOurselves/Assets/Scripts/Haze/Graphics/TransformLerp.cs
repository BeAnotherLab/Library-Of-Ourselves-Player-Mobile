using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLerp : MonoBehaviour {
	
	[SerializeField] Vector3 startAngles;
	[SerializeField] Vector3 endAngles;
	[SerializeField] Vector3 startScale = new Vector3(1, 1, 1);
	[SerializeField] Vector3 endScale = new Vector3(1, 1, 1);
	
	Transform t;
	
	void Start(){
		t = transform;
	}
	
	public void Lerp(float t){
		this.t.localEulerAngles = Vector3.Lerp(startAngles, endAngles, t);
		this.t.localScale = Vector3.Lerp(startScale, endScale, t);
	}
	
}
