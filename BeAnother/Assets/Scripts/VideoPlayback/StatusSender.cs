using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSender : MonoBehaviour {
	
	IEnumerator Start(){
		Sender sender = GetComponent<Sender>();
		//send status every couple seconds
		while(true){
			int fps = (int)(1.0f / Time.smoothDeltaTime);
			int battery = (int)(SystemInfo.batteryLevel * 100);
			sender.Send(fps+" "+battery);
			yield return new WaitForSeconds(2.5f);
		}
	}
	
}
