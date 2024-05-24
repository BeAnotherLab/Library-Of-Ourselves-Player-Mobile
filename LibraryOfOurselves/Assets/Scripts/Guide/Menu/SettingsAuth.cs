using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsAuth : MonoBehaviour
{
	public static bool temporalUnlock; //only true until closing the app

	[SerializeField] private InputField _pinInput;
	[SerializeField] private UnityEvent _openSettingsPanel;
	[SerializeField] private bool _shouldOpenSettings;

	private bool IsAppUnlocked = true;

	public static string CurrentPIN {
		get {
			if (HazePrefs.HasKey("settings-pin")) {
				return HazePrefs.GetString("settings-pin");
			} else return "000101";
		}
		set {
			HazePrefs.SetString("settings-pin", value);
		}
	}

	private void Start() {
		temporalUnlock = false;
		if (!IsAppUnlocked) OpenAuthPanel(false);
		else CloseAuthPanel();
	}

	public void OpenAuthPanel(bool openSettingsAfter) {
		if (openSettingsAfter && IsAppUnlocked && temporalUnlock) {
			_openSettingsPanel.Invoke(); //we're already good
			return;
		}
		transform.GetChild(0).gameObject.SetActive(true);
		_pinInput.text = "";
		_shouldOpenSettings = openSettingsAfter;
	}

	public void CloseAuthPanel() {
		if (IsAppUnlocked) transform.GetChild(0).gameObject.SetActive(false);
	}

	public void Auth() {
		if (CurrentPIN == _pinInput.text) { 
			IsAppUnlocked = true;
			temporalUnlock = true;
			
			foreach (ConnectionsDisplayer.DisplayedConnectionHandle h in ConnectionsDisplayer.Instance.Handles) h.display.UpdateDisplay(); //Update all device displays
			if (_shouldOpenSettings) _openSettingsPanel.Invoke();
			
			CloseAuthPanel();
			ConnectionsDisplayer.UpdateAllDisplays();
			if (VideoDisplay.expandedDisplay != null) VideoDisplay.expandedDisplay.Expand();//update that display.
		}
	}
}
