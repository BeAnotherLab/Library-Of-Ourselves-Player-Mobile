using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction1FromLanguage : MonoBehaviour {
	
	void Start(){
		GetComponent<Text>().text = Lang.GetText("instruction1") + CurrentSelection.Objects;
	}
	
}
