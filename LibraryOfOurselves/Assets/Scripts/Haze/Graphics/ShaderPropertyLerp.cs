using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPropertyLerp : MonoBehaviour {
	
	[SerializeField] string property = "_Amplitude";
	[SerializeField] float start = 0;
	[SerializeField] float end = 1;
	[SerializeField] Material mat;
	
	int propertyID;
	
	void Start(){
		propertyID = Shader.PropertyToID(property);
		mat.SetFloat(propertyID, start);
	}
	
	public void Lerp(float t){
		mat.SetFloat(propertyID, Mathf.Lerp(start, end, t));
	}
	
}
