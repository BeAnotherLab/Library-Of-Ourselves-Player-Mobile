using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackButton : MonoBehaviour {
	
	[SerializeField] UnityEvent onBackButton;
	[SerializeField] bool verbose = false;
	
	void Update(){
		
		if(Input.GetKeyDown(KeyCode.Escape)){
			Input.ResetInputAxes();
			
			if(onBackButton != null)
				GoBack();
			if(verbose)
				print("Pressed back button.");
		}
	}
	
	public void GoBack(){
		onBackButton.Invoke();
	}
	
}
