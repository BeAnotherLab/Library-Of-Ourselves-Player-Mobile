using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatShaderPropertyDynamicModifier : MonoBehaviour {
	
	[SerializeField] string propertyName = "_Float";
	[SerializeField] float inputMin = 0;
	[SerializeField] float inputMax = 1;
	[SerializeField] float outputMin = 0;
	[SerializeField] float outputMax = 1;
	[SerializeField] Material mat;
	
	int propertyId = -1;
	
	void Start(){
		if(propertyId != -1) return;
		propertyId = Shader.PropertyToID(propertyName);
	}
	
	public void Modify(float f){
		Start();
		mat.SetFloat(propertyId, Utilities.Map(inputMin, inputMax, outputMin, outputMax, f));
	}
	
}
