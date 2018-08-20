using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using extOSC;

[RequireComponent(typeof(Sender))]
[RequireComponent(typeof(Listener))]
public class Autoconnect : MonoBehaviour {
	
	[SerializeField] OSCTransmitter transmitter = null;
	[SerializeField] UnityEvent then = null;
	
	Sender sender;
	
	bool receivedPong;
	bool sentPong;
	
	IEnumerator Start(){
		
		if(transmitter == null) transmitter = FindObjectOfType(typeof(OSCTransmitter)) as OSCTransmitter;
		sender = GetComponent<Sender>();
		
		yield return Scan();
	}
	
	public IEnumerator Scan(){
		
		if(transmitter.IsAvaible) transmitter.Close();
		
		string localHost = OSCUtilities.GetLocalHost();
		
		//assign local ip with 255 instead of last number
		string ip = localHost;
		string[] subs = ip.Split('.');
		ip = "";
		for(int i = 0; i<subs.Length-1; ++i)
			ip += subs[i] + ".";
		ip += "255";
		transmitter.RemoteHost = ip;
		transmitter.Connect();
		
		receivedPong = false;
		sentPong = false;
		
		//broadcast "ping <ip>" over and over until it reaches
		while(!receivedPong || !sentPong){
			sender.Send("ping " + localHost);
			print("Received pong: " + receivedPong + ", sent pong: " + sentPong);
			yield return new WaitForSeconds(0.5f);
		}
		
		//everyone is connected!
		print("Autoconnect successful. Connected to " + transmitter.RemoteHost + ".");
		
		if(then != null)
			then.Invoke();
		
	}
	
	public void OnReceive(string msg){
		if(msg == "pong"){
			//we're good!
			receivedPong = true;
		}else{
			string[] subs = msg.Split(' ');
			if(subs.Length >= 2){
				if(subs[0] == "ping"){
					//then subs[1] is remote ip; connect to it
					print("Connecting to " + subs[1]);
					transmitter.RemoteHost = subs[1];
					//send PONG
					sender.Send("pong");
					sentPong = true;
				}else{
					Debug.LogError("Should have received ping with an ip but received " + msg + " instead..");
				}
			}else{
				Debug.LogError("Should have received ping with an ip but received " + msg + " instead..");
			}
		}
	}
	
}
