using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrderedEvent : MonoBehaviour {
	
	[SerializeField] UnityEvent first;
	[SerializeField] UnityEvent second;
	
	public void Invoke(){
		first.Invoke();
		second.Invoke();
	}
	
}
