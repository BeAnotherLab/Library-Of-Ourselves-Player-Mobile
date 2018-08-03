using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatusText : MonoBehaviour {
	
	[SerializeField] Text fps;
	[SerializeField] Text battery;
	
	public void OnStatusReceive(string data){
		string[] dat = data.Split(' ');
		if(dat.Length > 1){
			fps.text = dat[0];
			battery.text = dat[1] + "%";
			
			int f;
			int b;
			if(Int32.TryParse(dat[0], out f) && Int32.TryParse(dat[1], out b)){
				if(f < 50){
					//woah, fps shouldn't go that low!!
					fps.color = new Color(1, 1, 0);
				}else{
					fps.color = new Color(0, 1, 0);
				}
				if(b < 20){
					//woah, battery needs to be charged!
					battery.color = new Color(1, 1, 0);
				}else{
					battery.color = new Color(0, 1, 0);
				}
			}else{
				Debug.LogError("Can't convert to ints: " + data);
			}
			
		}else{
			Debug.LogError("Weird data received in beanother/status: " +data);
		}
	}
	
}
