using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {
	
	public static float Map(float inputMin, float inputMax, float outputMin, float outputMax, float value){
		return (value-inputMin)*(outputMax-outputMin) / (inputMax-inputMin) + outputMin;
	}
	
}
