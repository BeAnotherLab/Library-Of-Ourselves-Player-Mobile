using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStatus0ld : MonoBehaviour
{

	[SerializeField] Text fpsDisplay;
	[SerializeField] Text batteryDisplay;

	private void Update() {
		fpsDisplay.text = ""+GuideAdapter.LastFPSReceived;
		batteryDisplay.text = GuideAdapter.LastBatteryReceived + "%";
	}
}
