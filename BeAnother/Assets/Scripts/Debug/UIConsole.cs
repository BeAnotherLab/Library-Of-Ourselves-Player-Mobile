using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour {
	
	static UIConsole instance = null;
	
	Text text;
	string t = "Console:\n";
	
	void Start(){
		text = GetComponent<Text>();
		if(text)
			text.text = t;
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
		
		t += message + "\n";
		if(text) text.text = t;
	}
	
	public static string Logs(){
		return instance.t;
	}
	
}
