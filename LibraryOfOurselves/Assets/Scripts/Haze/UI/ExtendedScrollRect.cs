using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ExtendedScrollRect : MonoBehaviour {
	
	ScrollRect sr;
	
	void Awake(){
		sr = GetComponent<ScrollRect>();
	}
	
	public void ScrollDownAfterOneFrame(){
		sr.ScrollDownAfterOneFrame();
	}
	
}
