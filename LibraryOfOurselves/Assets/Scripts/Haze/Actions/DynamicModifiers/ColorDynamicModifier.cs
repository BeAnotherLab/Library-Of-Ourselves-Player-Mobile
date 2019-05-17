using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDynamicModifier : MonoBehaviour {
	
	[SerializeField] bool modifyR = false;
	[SerializeField] bool modifyG = false;
	[SerializeField] bool modifyB = false;
	[SerializeField] bool modifyA = true;
	[SerializeField] float constantR = 0;
	[SerializeField] float constantG = 0;
	[SerializeField] float constantB = 0;
	[SerializeField] float constantA = 1;
	[SerializeField] float inputMin = 0;
	[SerializeField] float inputMax = 1;
	[SerializeField] float outputMin = 0;
	[SerializeField] float outputMax = 1;
	[SerializeField] ColorEvent output;
	
	public void Modify(float f){
		f = Utilities.Map(inputMin, inputMax, outputMin, outputMax, f);
		output.Invoke(new Color(modifyR? f : constantR, modifyG? f : constantG, modifyB? f : constantB, modifyA? f : constantA));
	}
}
