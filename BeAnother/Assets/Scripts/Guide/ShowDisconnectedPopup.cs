using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDisconnectedPopup : MonoBehaviour {
	
	[SerializeField] GameObject popup;
	
	GameObject currentPopup = null;
	
	public void ShowPopup(){
		if(currentPopup == null){
			//find canvas:
			Transform canvas = FindObjectOfType<Canvas>().transform;
			currentPopup = GameObject.Instantiate(popup, canvas);
		}
	}
	
	public void HidePopup(){
		if(currentPopup != null){
			GameObject.Destroy(currentPopup);
			currentPopup = null;
		}
	}
	
}
