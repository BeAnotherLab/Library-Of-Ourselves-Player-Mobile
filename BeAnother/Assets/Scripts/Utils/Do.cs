using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Do : MonoBehaviour {
	
	[SerializeField] UnityEvent action;
	
	public void Go(){
		action.Invoke();
	}
	
}
