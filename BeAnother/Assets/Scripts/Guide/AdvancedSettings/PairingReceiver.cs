using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sender))]
public class PairingReceiver : MonoBehaviour {
	
	Sender sender;
	
	void Start(){
		sender = GetComponent<Sender>();
	}
	
	public void OnReceive(string data){
		if(data == "pair"){
			string key = Hashing.Adler32(SystemInfo.deviceUniqueIdentifier);
			Pairing.Pair = key;
			sender.Send(key);
		}else if(data == "unpair"){
			Pairing.Pair = null;
		}
	}
	
}
