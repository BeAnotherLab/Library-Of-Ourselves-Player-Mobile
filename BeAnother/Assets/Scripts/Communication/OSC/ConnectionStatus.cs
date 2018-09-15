using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Sender))]
public class ConnectionStatus : MonoBehaviour {
	
	static ConnectionStatus instance = null;
	public static ConnectionStatus Instance{
		get{ return instance; }
	}
	
	[SerializeField] float sendEvery = 1.0f;//send connection update every so often
	[SerializeField] float timeout = 5.0f;//if we spend this amount of time without receiving any connection status we'll consider we disconnected
	[SerializeField] bool verbose = false;
	[SerializeField] UnityEvent onDisconnection;
	[SerializeField] UnityEvent onReconnection;
	[SerializeField] bool active = true;//if false, don't mind querying connection status at all
	
	Sender sender;
	float lastReceived;
	bool disconnected = false;
	
	public bool Active{
		set{ active = value; if(active) lastReceived = Time.time; }
	}
	
	void onDisconnect(){
		if(!disconnected){
			disconnected = true;
			
			if(verbose) print("Disconnected.");
			
			onDisconnection.Invoke();
		}
	}
	
	void onReconnect(){
		if(disconnected){
			disconnected = false;
			
			if(verbose) print("Reconnected.");
			
			onReconnection.Invoke();
		}
	}
	
	IEnumerator Start(){
		instance = this;
		//DontDestroyOnLoad(gameObject);
		sender = GetComponent<Sender>();
		
		lastReceived = Time.time;
		
		while(true){
			yield return new WaitForSeconds(sendEvery);
			if(active){
				if(Time.time - lastReceived > timeout){
					//disconnected!
					onDisconnect();
				}
				sender.Send("1");
			}
		}
	}
	
	public void OnReceive(string dat){
		if(dat == "1"){
			lastReceived = Time.time;
			onReconnect();
		}else{
			Debug.LogError("Expecting connection data to be '1' but received " + dat);
		}
	}
	
	public void TrashObject(){
		instance = null;
		Object.Destroy(gameObject);
	}
	
}
