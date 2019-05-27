using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Messages : MonoBehaviour {
	
	[SerializeField] GameObject messagePrefab;
	[SerializeField] UnityEvent onReceiveMessage;
	
	public void PushMessage(string msg){
		GameObject message = Instantiate(messagePrefab, transform, false);
		Text text = message.GetComponent<Text>();
		if(text){
			text.text = msg;
		}
		onReceiveMessage.Invoke();
	}
	
	public void PushMessage(string msg, Color colour){
		GameObject message = Instantiate(messagePrefab, transform, false);
		Text text = message.GetComponent<Text>();
		if(text){
			text.text = msg;
			text.color = colour;
		}
		onReceiveMessage.Invoke();
	}
	
}
