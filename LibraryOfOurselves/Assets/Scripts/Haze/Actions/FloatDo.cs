using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatDo : MonoBehaviour {
	
	[SerializeField] bool onStart = false;
	[SerializeField] float def = 0;
	[SerializeField] FloatEvent action;
	
	void Start(){
		if(onStart) Go(def);
	}
	
	public void Go(float f){
		action.Invoke(f);
	}
	
}
