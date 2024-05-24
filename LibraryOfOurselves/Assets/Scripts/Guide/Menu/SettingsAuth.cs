using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsAuth : MonoBehaviour
{
	public static bool unlocked; 

	[SerializeField] private InputField _pinInput;
	[SerializeField] private UnityEvent _openSettingsPanel;
	[SerializeField] private GameObject _authFrame;
	public static void SetPin(string pin)
	{
		HazePrefs.SetString("settings-pin", pin);
	}
	
	public void OpenAuthPanel() {
		if (unlocked) //if we're already unlocked
		{
			_openSettingsPanel.Invoke(); //we're already good
		}
		else
		{
			_authFrame.SetActive(true);
			_pinInput.text = "";
		}
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
			_openSettingsPanel.Invoke();
			//TODO why do we need to update that display? 
			if (VideoDisplay.expandedDisplay != null) VideoDisplay.expandedDisplay.Expand();//update that display.
		}
	}
}
