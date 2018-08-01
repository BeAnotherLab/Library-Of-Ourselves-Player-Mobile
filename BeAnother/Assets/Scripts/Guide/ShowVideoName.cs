using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowVideoName : MonoBehaviour {
	
	void Start(){
		GetComponent<Text>().text = CurrentSelection.Name;
	}
	
}
