using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using extOSC;

public class Listener : MonoBehaviour {
	
	[SerializeField] string address = "/beanother";
	[SerializeField] StringEvent onReceive;
	[SerializeField] OSCReceiver receiver = null;
	[SerializeField] bool verbose = false;
	
	void Start(){
		OSCReceiver r = receiver != null ? receiver : FindObjectOfType(typeof(OSCReceiver)) as OSCReceiver;
		if(r)
			r.Bind(address, receivedMessage);
	}
	
	void receivedMessage(OSCMessage message){
		if(!enabled) return;
		
		string msg = message.GetValues(OSCValueType.String)[0].StringValue;
		
		if(verbose) print("Received on " + address + ": " + msg);
		
		if(onReceive != null) onReceive.Invoke(msg);
	}
	
}
