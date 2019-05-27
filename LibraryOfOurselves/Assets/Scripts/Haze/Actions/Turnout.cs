using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Turnout : MonoBehaviour {
	
	[SerializeField] List<UnityEvent> events;
	
	UnityEvent next = null;
	
	public void Switch(int nextEvent){
		next = events[nextEvent];
	}
	
	public void Proceed(){
		if(next == null) StartCoroutine(track());
		else{
			next.Invoke();
			next = null;
		}
	}
	
	IEnumerator track(){
		while(next == null)
			yield return null;
		next.Invoke();
		next = null;
	}
	
}
