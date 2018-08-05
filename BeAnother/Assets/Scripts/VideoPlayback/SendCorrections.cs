using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCorrections : MonoBehaviour {
	
	public void Send(){
		GetComponent<Sender>().Send("rotate " + CurrentSelection.Angles.x + " " + CurrentSelection.Angles.y + " " + CurrentSelection.Angles.z);
	}
	
}
