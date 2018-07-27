using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour {
	
	static UIConsole instance = null;
	
	Text text;
	
	void Start(){
		text = GetComponent<Text>();
		text.text = "Console:\n";
	}
	
	void OnEnable(){
		if(instance == null) instance = this;
		else gameObject.SetActive(false);
		
		Application.logMessageReceived += HandleLog;
	}
	
	void OnDisable(){
		if(instance == this) instance = null;
		
		Application.logMessageReceived -= HandleLog;
	}
	
	void HandleLog (string message, string stackTrace, LogType type){
		if(!enabled) return;
		
		text.text += message + "\n";
	}
	
}
