using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Do : MonoBehaviour {
	
	[SerializeField] bool onStart = false;
	[SerializeField] bool onEnable = false;
	[SerializeField] bool onDisable = false;
	[SerializeField] UnityEvent action;
	
	void Start(){
		if(onStart) Go();
	}

	private void OnEnable() {
		if(onEnable) Go();
	}

	private void OnDisable() {
		if(onDisable) Go();
	}

	public void Go(){
		action.Invoke();
	}
	
}
