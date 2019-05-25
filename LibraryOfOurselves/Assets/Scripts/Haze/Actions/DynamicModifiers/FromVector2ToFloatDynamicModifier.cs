using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haze;

public class FromVector2ToFloatDynamicModifier : MonoBehaviour {

	[SerializeField] Vector2Element modifier = Vector2Element.X;
	[SerializeField] float inputMinimum = 0;
	[SerializeField] float inputMaximum = 1;
	[SerializeField] float outputMinimum = 0;
	[SerializeField] float outputMaximum = 1;
	[SerializeField] FloatEvent output;
	[SerializeField] bool verbose = false;

	enum Vector2Element {
		X, Y
	}

	public void Modify(Vector2 vec2) {
		float val = modifier == Vector2Element.X ? vec2.x : vec2.y;
		if(verbose) Debug.Log("Input: " + val);
		val = Utilities.Map(inputMinimum, inputMaximum, outputMinimum, outputMaximum, Mathf.Clamp(val, inputMinimum, inputMaximum));
		if(verbose) Debug.Log("Output: " + val);
		output.Invoke(val);
	}

}
