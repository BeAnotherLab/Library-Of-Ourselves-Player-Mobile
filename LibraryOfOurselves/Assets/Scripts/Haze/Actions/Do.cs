using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Do : MonoBehaviour {
	
	[SerializeField] bool onStart = false;
	[SerializeField] UnityEvent action;
	
	void Start(){
		if(onStart) Go();
	}
	
	public void Go(){
		action.Invoke();
	}
	
}
