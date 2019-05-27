using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColourLerp : MonoBehaviour {
	
	[SerializeField] Color start = Color.white;
	[SerializeField] Color end = Color.white;
	
	TextMesh tm;
	
	void Start(){
		tm = GetComponent<TextMesh>();
	}
	
	public void Lerp(float t){
		tm.color = Color.Lerp(start, end, t);
	}
	
}
