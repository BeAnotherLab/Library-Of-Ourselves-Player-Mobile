using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCurrentObjects : MonoBehaviour{

	private void OnEnable() {
		if(GuideVideoPlayer.Instance && GuideVideoPlayer.Instance.CurrentVideo != null) {
			GetComponent<Text>().text = "\t\t" + GuideVideoPlayer.Instance.CurrentVideo.Settings.objectsNeeded;
		} else {
			GetComponent<Text>().text = "\t\t" + VideoDisplay.expandedDisplay.Settings.objectsNeeded;
		}
	}

}
