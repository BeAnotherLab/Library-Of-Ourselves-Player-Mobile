using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCurrentTitle : MonoBehaviour{

	private void OnEnable() {
		GetComponent<Text>().text = VideoDisplay.expandedDisplay.VideoName;
	}

}
