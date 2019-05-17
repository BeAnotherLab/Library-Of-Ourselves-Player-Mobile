using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour {
	
	#if UNITY_EDITOR
	[SerializeField] bool In = false;
	
	void OnTriggerEnter2D(Collider2D c){
		if(c.CompareTag("Player"))
			In = true;
	}
	
	void OnTriggerExit2D(Collider2D c){
		if(c.CompareTag("Player"))
			In = false;
	}
	
	void Update(){
		if(In){
			//do something to prevent compile warning lol
			bool inside = In;
			In = inside;
		}
	}
	#endif
	
}
