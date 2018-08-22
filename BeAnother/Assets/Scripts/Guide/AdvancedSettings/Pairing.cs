using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pairing {
	
	public static string Pair{
		get{
			if(PlayerPrefs.HasKey("pair")){
				if(PlayerPrefs.GetString("pair") == "") return null;
				else return PlayerPrefs.GetString("pair");
			}else return null;
		}
		set{
			if(value == null || value == "") PlayerPrefs.SetString("pair", "");
			else PlayerPrefs.SetString("pair", value);
		}
	}
	
}
