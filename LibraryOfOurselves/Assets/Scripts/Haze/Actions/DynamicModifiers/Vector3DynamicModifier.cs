using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3DynamicModifier : MonoBehaviour {
	
	[SerializeField] bool modifyX = true;
	[SerializeField] bool modifyY = true;
	[SerializeField] bool modifyZ = true;
	[SerializeField] float constantX = 0;
	[SerializeField] float constantY = 0;
	[SerializeField] float constantZ = 0;
	[SerializeField] float inputMin = 0;
	[SerializeField] float inputMax = 1;
	[SerializeField] float outputMin = 0;
	[SerializeField] float outputMax = 1;
	[SerializeField] Vector3Event output;
	
	public void Modify(float f){
		f = Utilities.Map(inputMin, inputMax, outputMin, outputMax, f);
		output.Invoke(new Vector3(modifyX? f : constantX, modifyY? f : constantY, modifyZ? f : constantZ));
	}
	
}
