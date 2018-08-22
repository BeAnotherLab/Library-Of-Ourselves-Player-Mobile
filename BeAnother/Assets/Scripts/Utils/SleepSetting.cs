using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSetting : MonoBehaviour {
	
	enum Sleep{
		Never,
		SystemSetting,
		CustomSeconds
	}
	
	[SerializeField] Sleep sleep;
	[SerializeField] int seconds = 60;
	
	void Start(){
		switch(sleep){
			case Sleep.Never: Screen.sleepTimeout = SleepTimeout.NeverSleep; break;
			case Sleep.SystemSetting: Screen.sleepTimeout = SleepTimeout.SystemSetting; break;
			case Sleep.CustomSeconds: Screen.sleepTimeout = seconds; break;
		}
	}
	
}
