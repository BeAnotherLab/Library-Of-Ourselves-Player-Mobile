using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTap : MonoBehaviour {
	
	[SerializeField] UnityEvent onTap;
	#if UNITY_EDITOR
	[SerializeField] bool verbose = false;
	#endif
	
	Vector3 mousePosition = Vector3.zero;
	
	void OnMouseDown(){
		mousePosition = Input.mousePosition;
	}
	
	void OnMouseUpAsButton(){
		//check how different the previous and current mouse positions are
		float distanceSqr = (Input.mousePosition - mousePosition).sqrMagnitude;
		if(distanceSqr > 100) return;//more than 10 pixels
		#if UNITY_EDITOR
		if(verbose) print("Tapped " + name);
		#endif
		onTap.Invoke();
	}
	
	public void AddListener(UnityAction action){
		onTap.AddListener(action);
	}
	
}
