using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haze;

public class DynamicModifier : MonoBehaviour {

	[SerializeField] float inputMin = 0;
	[SerializeField] float inputMax = 1;
	[SerializeField] float outputMin = 0;
	[SerializeField] float outputMax = 1;
	[SerializeField] FloatEvent output;

	public void Modify(float f) {
		f = Utilities.Map(inputMin, inputMax, outputMin, outputMax, f);
		output.Invoke(f);
	}

}
