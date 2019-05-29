using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCurrentTitle : MonoBehaviour{

	private void OnEnable() {
		if(GuideVideoPlayer.Instance && GuideVideoPlayer.Instance.CurrentVideo != null) {
			GetComponent<Text>().text = GuideVideoPlayer.Instance.CurrentVideo.VideoName;
		} else {
			GetComponent<Text>().text = VideoDisplay.expandedDisplay.VideoName;
		}
	}

}
