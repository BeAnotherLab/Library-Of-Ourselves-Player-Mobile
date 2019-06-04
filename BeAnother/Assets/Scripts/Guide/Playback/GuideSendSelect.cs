using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSendSelect : MonoBehaviour {
	
	public void Send(){
		if(CurrentSelection.Is360)
			GetComponent<Sender>().Send("select " + CurrentSelection.Name + " 360");
		else
			GetComponent<Sender>().Send("select " + CurrentSelection.Name + " 235");
	}
	
}
