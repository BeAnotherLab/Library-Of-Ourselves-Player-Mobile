using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger2D : MonoBehaviour {
	
	[SerializeField] new string tag = "Player";
	[SerializeField] bool onlyOnce = false;
	public UnityEvent onEnter = new UnityEvent();
	public UnityEvent onExit = new UnityEvent();
	[SerializeField] bool verbose = false;
	
	int within = 0;
	bool preventEntering = false;
	bool preventExiting = false;
	
	void OnTriggerEnter2D(Collider2D collider){
		if(preventEntering) return;
		if(collider.CompareTag(tag)){
			++within;
			if(within == 1){
				onEnter.Invoke();
				if(onlyOnce) preventEntering = true;
				if(verbose) print("Entered " + name);
				
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		if(preventExiting) return;
		if(collider.CompareTag(tag)){
			--within;
			if(within == 0){
				onExit.Invoke();
				if(onlyOnce) preventExiting = true;
				if(verbose) print("Exited " + name);
			}
		}
	}
	
}
