using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using extOSC;

public class Listener : MonoBehaviour {
	
	[SerializeField] string address = "/beanother";
	[SerializeField] StringEvent onReceive;
	[SerializeField] OSCReceiver receiver = null;
	[SerializeField] bool bindOnStart = true;
	[SerializeField] bool verbose = false;
	
	bool bound = false;
	
	public string Address{
		get{ return address; }
		//cannot change Address once the listener is already bound. Uncheck bindOnStart then call Bind() yourself to change address through code.
	}
	
	void Start(){
		if(bindOnStart){
			Bind();
		}
	}
	
	public void Bind(string address = ""){
		if(bound){
			Debug.LogError("Cannot bind Listener to multiple addresses.");
			return;
		}
		bound = true;
		if(address == "") address = this.address;
		receiver = receiver != null ? receiver : FindObjectOfType(typeof(OSCReceiver)) as OSCReceiver;
		receiver.Bind(address, receivedMessage);
		this.address = address;
	}
	
	void receivedMessage(OSCMessage message){
		if(!enabled) return;
		
		string msg = message.GetValues(OSCValueType.String)[0].StringValue;
		
		if(verbose) print("Received on " + address + ": " + msg);
		
		if(onReceive != null) onReceive.Invoke(msg);
	}
	
}
