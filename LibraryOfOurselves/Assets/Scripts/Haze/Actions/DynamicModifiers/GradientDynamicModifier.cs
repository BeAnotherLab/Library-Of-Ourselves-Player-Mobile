using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientDynamicModifier : MonoBehaviour{

	[SerializeField] Gradient gradient;
	[SerializeField] float inputMin = 0;
	[SerializeField] float inputMax = 1;
	[SerializeField] ColorEvent output;

	public void Modify(float f) {
		f = Utilities.Map(inputMin, inputMax, 0, 1, f);
		output.Invoke(gradient.Evaluate(f));
	}

}
