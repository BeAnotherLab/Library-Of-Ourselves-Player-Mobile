using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Settings : MonoBehaviour {

	[SerializeField] InputField newPinField;
	[SerializeField] UnityEvent onChangePin;
	[SerializeField] Slider allowedErrorSlider;
	[SerializeField] Slider maximumErrorSlider;
	[SerializeField] Slider syncedPlaybackSlider;
	[SerializeField] Slider syncTimeSlider;
	[SerializeField] Text allowedErrorText;
	[SerializeField] Text maximumErrorText;
	[SerializeField] Text syncedPlaybackText;
	[SerializeField] Text syncTimeText;

	public static Settings Instance { get; private set; }

	public static bool SendSyncMessages { get; private set; } = true;

	public void OnToggleSyncPackets(bool toggle) {
		SendSyncMessages = toggle;
	}

	public void OnClickChangePin() {
		SettingsAuth.CurrentPIN = newPinField.text.Length > 0 ? newPinField.text : "0000";
		onChangePin.Invoke();
	}

	public void OnChangeLanguage() {
		if(VideoDisplay.expandedDisplay != null)
			VideoDisplay.expandedDisplay.expand();
	}

	public void DisableAdminAccess() {
		SettingsAuth.ShutoffAdminAccess();
	}

	public static float AllowedErrorForSyncedPlayback {
		get {
			if(HazePrefs.HasKey("allowederrorforsyncedplayback"))
				return HazePrefs.GetFloat("allowederrorforsyncedplayback");
			else return 0.25f;
		}
		set {
			HazePrefs.SetFloat("allowederrorforsyncedplayback", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float MaximumErrorForSyncedPlayback {
		get {
			if(HazePrefs.HasKey("maximumallowederrorforsyncedplayback"))
				return HazePrefs.GetFloat("maximumallowederrorforsyncedplayback");
			else return 1;
		}
		set {
			HazePrefs.SetFloat("maximumallowederrorforsyncedplayback", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float SyncedPlaybackMaximumTimeDilation {
		get {
			if(HazePrefs.HasKey("maximumtimedilationforsyncedplayback"))
				return HazePrefs.GetFloat("maximumtimedilationforsyncedplayback");
			else return 1.5f;
		}
		set {
			HazePrefs.SetFloat("maximumtimedilationforsyncedplayback", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float SyncTime {
		get {
			if(HazePrefs.HasKey("synctime")) {
				return HazePrefs.GetFloat("synctime");
			} else return 1.5f;
		}
		set {
			HazePrefs.SetFloat("synctime", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public void ResetSyncSettings() {
		AllowedErrorForSyncedPlayback = 0.25f;
		MaximumErrorForSyncedPlayback = 1.0f;
		SyncedPlaybackMaximumTimeDilation = 1.5f;
		SyncTime = 1.5f;
		OnEnable();
	}

	private void OnEnable() {
		allowedErrorSlider.SetValueWithoutNotify(AllowedErrorForSyncedPlayback);
		maximumErrorSlider.SetValueWithoutNotify(MaximumErrorForSyncedPlayback);
		syncedPlaybackSlider.SetValueWithoutNotify(SyncedPlaybackMaximumTimeDilation);
		syncTimeSlider.SetValueWithoutNotify(SyncTime);

		allowedErrorText.text = "Allowed error (" + AllowedErrorForSyncedPlayback + "s)";
		maximumErrorText.text = "Maximum error (" + MaximumErrorForSyncedPlayback + "s)";
		syncedPlaybackText.text = "Max time dilation (x" + SyncedPlaybackMaximumTimeDilation + ")";
		syncTimeText.text = "Sync time (" + SyncTime + "s)";

		allowedErrorSlider.maxValue = MaximumErrorForSyncedPlayback;
		maximumErrorSlider.minValue = AllowedErrorForSyncedPlayback;
	}

	private void Awake() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}

	public void setAllowedError(float f) {
		AllowedErrorForSyncedPlayback = f;
	}

	public void setMaxError(float f) {
		MaximumErrorForSyncedPlayback = f;
	}

	public void setTimeDilation(float f) {
		SyncedPlaybackMaximumTimeDilation = f;
	}

	public void setSyncTime(float f) {
		SyncTime = f;
	}

}
