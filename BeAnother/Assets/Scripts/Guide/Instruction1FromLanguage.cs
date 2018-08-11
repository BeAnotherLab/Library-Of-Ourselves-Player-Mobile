using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction1FromLanguage : MonoBehaviour {
	
	void Start(){
		GetComponent<Text>().text = Lang.GetText("instructions") + "\n\n" + Lang.GetText("instruction1") + CurrentSelection.Objects;
		for(int i = 2; i<=6; ++i){
			GetComponent<Text>().text += "\n" + Lang.GetText("instruction"+i);
		}
	}
	
}
