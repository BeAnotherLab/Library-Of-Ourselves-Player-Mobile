using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PushText : MonoBehaviour {
	
	Text text;
	
	void Awake(){
		text = GetComponent<Text>();
	}
	
	public void Push(string str){
		text.text += "\n" + str;
	}
	
}
