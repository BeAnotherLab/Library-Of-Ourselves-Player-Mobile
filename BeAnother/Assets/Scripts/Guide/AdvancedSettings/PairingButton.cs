using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Sender))]
public class PairingButton : MonoBehaviour {
	
	[SerializeField] IncreaseFillAmount lockAnimation;
	[SerializeField] UnityEvent onPair;
	[SerializeField] UnityEvent onUnpair;
	
	Sender sender;
	
	void Start(){
		sender = GetComponent<Sender>();
		if(Pairing.Pair == null){
			lockAnimation.MinOut();
		}else{
			lockAnimation.MaxOut();
		}
	}
	
	public void OnTap(){
		if(Pairing.Pair == null){
			//attempt to pair with currently connected player
			sender.Send("pair");
			lockAnimation.Increase();
		}else{
			//unpair
			Pairing.Pair = null;
			sender.Send("unpair");
			onUnpair.Invoke();
			lockAnimation.Decrease();
		}
	}
	
	public void OnReceive(string data){
		//the data we receive is the id we requested of the newly paired device
		Pairing.Pair = data;
		if(Pairing.Pair != null)
			onPair.Invoke();
		else onUnpair.Invoke();//assumption should be that this won't get called.
	}
	
}
