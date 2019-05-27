using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2DynamicModifier : MonoBehaviour {
	
	[SerializeField] bool modifyX = true;
	[SerializeField] bool modifyY = true;
	[SerializeField] float constantX = 0;
	[SerializeField] float constantY = 0;
	[SerializeField] float inputMin = 0;
	[SerializeField] float inputMax = 1;
	[SerializeField] float outputMin = 0;
	[SerializeField] float outputMax = 1;
	[SerializeField] Vector2Event output;
	
	public void Modify(float f){
		f = Utilities.Map(inputMin, inputMax, outputMin, outputMax, f);
		output.Invoke(new Vector2(modifyX? f : constantX, modifyY? f : constantY));
	}
	
}
