using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowVideoName : MonoBehaviour {
	
	[SerializeField] Text objects = null;
	
	void Start(){
		GetComponent<Text>().text = CurrentSelection.Name;
		if(objects != null)
			objects.text = CurrentSelection.Objects;
	}
	
}
