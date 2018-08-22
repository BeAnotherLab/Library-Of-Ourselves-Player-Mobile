using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class Sender : MonoBehaviour {
	
	[SerializeField] string address = "/beanother";
	[SerializeField] string defaultMessage = "";
	[SerializeField] OSCTransmitter transmitter;
	[SerializeField] bool verbose = false;
	
	public string Address{
		get{ return address; }
		set{ address = value; }
	}
	
	void Start(){
		if(transmitter == null)
			transmitter = FindObjectOfType(typeof(OSCTransmitter)) as OSCTransmitter;
		
		//if there is a default message, attach this to the button if there is one
		if(defaultMessage != ""){
			Button button = GetComponent<Button>();
			if(button && button.onClick.GetPersistentEventCount() <= 0){
				button.onClick.AddListener(Send);
			}
		}
	}
	
	public void Send(string msg){
		if(!enabled) return;
		
		var message = new OSCMessage(address);
		message.AddValue(OSCValue.String(msg));
		if(transmitter)
			transmitter.Send(message);
		else if(verbose){
			print("Dummy sent on " + address + ": " + msg);
			return;
		}
		
		if(verbose) print("Sent on " + address + ": " + msg);
		
	}
	
	public void Send(){
		Send(defaultMessage);
	}
	
}
