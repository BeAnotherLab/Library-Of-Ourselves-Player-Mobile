using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorShaderPropertyDynamicModifier : MonoBehaviour {

	[SerializeField] string propertyName = "_Color";
	[SerializeField] bool modifyR = false;
	[SerializeField] bool modifyG = false;
	[SerializeField] bool modifyB = false;
	[SerializeField] bool modifyA = true;
	[SerializeField] float constantR = 0;
	[SerializeField] float constantG = 0;
	[SerializeField] float constantB = 0;
	[SerializeField] float constantA = 1;
	[SerializeField] Material mat;
	
	int propertyId = -1;
	
	void Start(){
		if(propertyId != -1) return;
		propertyId = Shader.PropertyToID(propertyName);
	}
	
	public void Modify(float f){
		Start();
		mat.SetColor(propertyId, new Color(modifyR? f : constantR, modifyG? f : constantG, modifyB? f : constantB, modifyA? f : constantA));
	}
	
}
