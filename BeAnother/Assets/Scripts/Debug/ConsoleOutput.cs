using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleOutput : MonoBehaviour {
	
	#if UNITY_EDITOR
	
	[SerializeField] string message = "Hello world";
	
	void OnEnable(){
		Debug.Log(message);
	}
	
	#endif
	
}
