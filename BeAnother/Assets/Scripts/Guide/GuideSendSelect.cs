﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSendSelect : MonoBehaviour {
	
	public void Send(){
		GetComponent<Sender>().Send("select " + CurrentSelection.Name + " 235");
	}
	
}