using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsAuth : MonoBehaviour{

	[SerializeField] InputField pinInput;
	[SerializeField] UnityEvent openSettingsPanel;

	bool shouldOpenSettings;
    
	public bool IsAppUnlocked {
		get {
			/*if(HazePrefs.HasKey("app-unlocked")) {
				if(HazePrefs.GetInt("app-unlocked") == 1) {
					return true;
				}
			}
			return false;*/
			return true; // App is always unlocked now. Might want to revert this for production build.
		}
		set {
			HazePrefs.SetInt("app-unlocked", value ? 1 : 0);
		}
	}

	public static bool TemporalUnlock { get; private set; } //only true until closing the app

	public static string CurrentPIN {
		get {
			if(HazePrefs.HasKey("settings-pin")) {
				return HazePrefs.GetString("settings-pin");
			} else return "000101";
		}
		set {
			HazePrefs.SetString("settings-pin", value);
		}
	}

	private void Start() {
		TemporalUnlock = false;
		if(!IsAppUnlocked) {
			OpenAuthPanel(false);
		} else {
			CloseAuthPanel();
		}
	}

	public void OpenAuthPanel(bool openSettingsAfter) {
		if(openSettingsAfter && IsAppUnlocked && TemporalUnlock) {
			openSettingsPanel.Invoke();//we're already good
			return;
		}
		transform.GetChild(0).gameObject.SetActive(true);
		pinInput.text = "";
		shouldOpenSettings = openSettingsAfter;
	}

	public void CloseAuthPanel() {
		if(IsAppUnlocked) {
			transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	public void Auth() {
		if(CurrentPIN == pinInput.text) {
			IsAppUnlocked = true;
			TemporalUnlock = true;
			//Update all device displays
			foreach(ConnectionsDisplayer.DisplayedConnectionHandle h in ConnectionsDisplayer.Instance.Handles) {
				h.display.UpdateDisplay();
			}
			//Ok!
			if(shouldOpenSettings) {
				openSettingsPanel.Invoke();
			}
			CloseAuthPanel();
			ConnectionsDisplayer.UpdateAllDisplays();
			if(VideoDisplay.expandedDisplay != null) {
				VideoDisplay.expandedDisplay.expand();//update that display.
			}
		}
	}

	public static void ShutoffAdminAccess() {
		TemporalUnlock = false;
	}

}
