using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCurrentObjects : MonoBehaviour{

	private void OnEnable() {
		GetComponent<Text>().text = "\t\t"+VideoDisplay.expandedDisplay.Settings.objectsNeeded;
	}

}
