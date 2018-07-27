using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class OSCInfo : MonoBehaviour {
	
	void Start(){
		
		OSCTransmitter transmitter = GetComponent<OSCTransmitter>();
		OSCReceiver receiver = GetComponent<OSCReceiver>();
		
		if(transmitter){
			print("OSC Transmitter: " + transmitter);
		}
		
		if(receiver){
			print("OSC Receiver: " + receiver);
		}
		
		print("Host: " + OSCUtilities.GetLocalHost());
		
	}
	
}
