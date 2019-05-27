
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Print : MonoBehaviour {
	
	public void SaySomething(){
		print(name + " is saying hello.");
	}
	
	public void SaySomething(string text){
		print(text);
	}
	
}