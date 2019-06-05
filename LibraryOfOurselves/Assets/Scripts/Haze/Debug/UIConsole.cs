using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour {
	
	static UIConsole instance = null;

	[SerializeField] bool bottomToTop = true;
	
	Text text;
	string t = "";
	
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

		if(bottomToTop)
			t = message + "\n" + t;
		else
			t = t + "\n" + message;

		if(text) text.text = "Console:\n"+t;
	}
	
	public static string Logs(){
		return instance.t;
	}
	
}
