using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackButton : MonoBehaviour {
	
	[SerializeField] List<GameObject> onlyIfAllInactive;
	[SerializeField] UnityEvent onBackButton;
	[SerializeField] bool verbose = false;
	
	void Update(){
		
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(!allInactive()) return;
			Input.ResetInputAxes();
			
			if(onBackButton != null)
				GoBack();
		}
	}
	
	bool allInactive(){
		foreach(GameObject go in onlyIfAllInactive){
			if(go.activeSelf) return false;
		}
		return true;
	}
	
	public void GoBack(){
		if(verbose)
			print("Pressed back button.");
		onBackButton.Invoke();
	}
	
}
