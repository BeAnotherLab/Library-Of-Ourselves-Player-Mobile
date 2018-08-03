using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSender : MonoBehaviour {
	
	IEnumerator Start(){
		Sender sender = GetComponent<Sender>();
		//send status every couple seconds
		while(true){
			int fps = (int)(1.0f / Time.smoothDeltaTime)+1;//i'm ceiling both of these values cos i think it makes more sense lol
			int battery = (int)(SystemInfo.batteryLevel * 100)+1;
			sender.Send(fps+" "+battery);
			yield return new WaitForSeconds(2.5f);
		}
	}
	
}
