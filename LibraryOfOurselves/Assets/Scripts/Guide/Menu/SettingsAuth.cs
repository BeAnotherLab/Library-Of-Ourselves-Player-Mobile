using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsAuth : MonoBehaviour
{
	public static bool unlocked = false; 

	[SerializeField] private InputField _pinInput;
	[SerializeField] private UnityEvent _openSettingsPanel;
	
	public static void SetPin(string pin)
	{
		HazePrefs.SetString("settings-pin", pin);
	}
	
	public void OpenAuthPanel() {
		if (unlocked) _openSettingsPanel.Invoke(); //we're already good
	}

	public void CloseAuthPanel() {
		transform.GetChild(0).gameObject.SetActive(false);
	}
	
	public void Auth()
	{
		string currentPin;
		if (HazePrefs.HasKey("settings-pin")) currentPin = HazePrefs.GetString("settings-pin");
		else currentPin = "000101";
		
		if (currentPin == _pinInput.text) { 
			unlocked = true;
			CloseAuthPanel();
			//TODO why do we need to update that display? 
			if (VideoDisplay.expandedDisplay != null) VideoDisplay.expandedDisplay.Expand();//update that display.
		}
	}
}
