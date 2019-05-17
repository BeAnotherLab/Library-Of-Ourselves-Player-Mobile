using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnValueReaches : MonoBehaviour {
	
	[SerializeField] float trigger;
	[SerializeField] bool triggerWhenValIsLower = false;
	[SerializeField] UnityEvent onValueReached;
	
	bool reached = false;
	
	void valueReached(){
		reached = true;
		onValueReached.Invoke();
	}
	
	public void OnValueChange(float val){
		if(reached) return;
		if(val >= trigger && !triggerWhenValIsLower)
			valueReached();
		else if(val <= trigger && triggerWhenValIsLower)
			valueReached();
	}
	
}
