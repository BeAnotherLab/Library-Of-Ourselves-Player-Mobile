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
	[SerializeField] Slider jumpAheadSlider;
	[SerializeField] Slider allowedUsersSlider;
	[SerializeField] Slider maxUsersSlider;
	[SerializeField] Slider syncedUsersSlider;
	[SerializeField] Text allowedErrorText;
	[SerializeField] Text maximumErrorText;
	[SerializeField] Text syncedPlaybackText;
	[SerializeField] Text syncTimeText;
	[SerializeField] Text jumpAheadText;
	[SerializeField] Text allowedUsersText;
	[SerializeField] Text maxUsersText;
	[SerializeField] Text syncedUsersText;
	[SerializeField] Toggle forceMultiUsersToggle;

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
			else return 0.1f;
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
			else return 1.8f;
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
			} else return 0.7f;
		}
		set {
			HazePrefs.SetFloat("synctime", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float JumpAheadTime {
		get {
			if(HazePrefs.HasKey("jumptime")) {
				return HazePrefs.GetFloat("jumptime");
			} else return 0;
		}
		set {
			HazePrefs.SetFloat("jumptime", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float AllowedErrorForSyncedPlaybackUsers {
		get {
			if(HazePrefs.HasKey("allowederrorforsyncedplaybackusers"))
				return HazePrefs.GetFloat("allowederrorforsyncedplaybackusers");
			else return 0.4f;
		}
		set {
			HazePrefs.SetFloat("allowederrorforsyncedplaybackusers", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float MaximumErrorForSyncedPlaybackUsers {
		get {
			if(HazePrefs.HasKey("maximumallowederrorforsyncedplaybackusers"))
				return HazePrefs.GetFloat("maximumallowederrorforsyncedplaybackusers");
			else return 0.8f;
		}
		set {
			HazePrefs.SetFloat("maximumallowederrorforsyncedplaybackusers", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static float SyncedPlaybackMaximumTimeDilationUsers {
		get {
			if(HazePrefs.HasKey("maximumtimedilationforsyncedplaybackusers"))
				return HazePrefs.GetFloat("maximumtimedilationforsyncedplaybackusers");
			else return 1.2f;
		}
		set {
			HazePrefs.SetFloat("maximumtimedilationforsyncedplaybackusers", value);
			GuideVideoPlayer.Instance.RetrieveSyncSettings();
			if(Instance) Instance.OnEnable();
		}
	}

	public static bool ForceMultiUserSetup {
		get {
			if(HazePrefs.HasKey("forcemultiusersetup")) {
				return HazePrefs.GetInt("forcemultiusersetup") == 1;
			} else return false;
		}
		set {
			HazePrefs.SetInt("forcemultiusersetup", value ? 1 : 0);
		}
	}



	public void ResetSyncSettings() {
		AllowedErrorForSyncedPlayback = 0.1f;
		MaximumErrorForSyncedPlayback = 1.0f;
		SyncedPlaybackMaximumTimeDilation = 1.8f;
		SyncTime = 0.7f;
		JumpAheadTime = 0;
		AllowedErrorForSyncedPlaybackUsers = 0.4f;
		MaximumErrorForSyncedPlayback = 0.8f;
		SyncedPlaybackMaximumTimeDilationUsers = 1.2f;
		OnEnable();
	}

	private void OnEnable() {
		allowedErrorSlider.SetValueWithoutNotify(AllowedErrorForSyncedPlayback);
		maximumErrorSlider.SetValueWithoutNotify(MaximumErrorForSyncedPlayback);
		syncedPlaybackSlider.SetValueWithoutNotify(SyncedPlaybackMaximumTimeDilation);
		syncTimeSlider.SetValueWithoutNotify(SyncTime);
		jumpAheadSlider.SetValueWithoutNotify(JumpAheadTime);
		allowedUsersSlider.SetValueWithoutNotify(AllowedErrorForSyncedPlaybackUsers);
		maxUsersSlider.SetValueWithoutNotify(MaximumErrorForSyncedPlaybackUsers);
		syncedUsersSlider.SetValueWithoutNotify(SyncedPlaybackMaximumTimeDilationUsers);

		allowedErrorText.text = "Allowed error (" + AllowedErrorForSyncedPlayback + "s)";
		maximumErrorText.text = "Maximum error (" + MaximumErrorForSyncedPlayback + "s)";
		syncedPlaybackText.text = "Max time dilation (x" + SyncedPlaybackMaximumTimeDilation + ")";
		syncTimeText.text = "Sync time (" + SyncTime + "s)";
		jumpAheadText.text = "Jump ahead time (" + JumpAheadTime + "s)";
		allowedUsersText.text = "Allowed error - multi users (" + AllowedErrorForSyncedPlaybackUsers + "s)";
		maxUsersText.text = "Maximum error - multi users (" + MaximumErrorForSyncedPlaybackUsers + "s)";
		syncedUsersText.text = "Max time dilation - multi users (x" + SyncedPlaybackMaximumTimeDilationUsers + ")";

		allowedErrorSlider.maxValue = MaximumErrorForSyncedPlayback;
		maximumErrorSlider.minValue = AllowedErrorForSyncedPlayback;
		allowedUsersSlider.maxValue = MaximumErrorForSyncedPlaybackUsers;
		maxUsersSlider.minValue = AllowedErrorForSyncedPlaybackUsers;

		forceMultiUsersToggle.onValueChanged.AddListener(delegate (bool val){
			ForceMultiUserSetup = val;
		});
		forceMultiUsersToggle.SetIsOnWithoutNotify(ForceMultiUserSetup);
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

	public void setJumpTime(float f) {
		JumpAheadTime = f;
	}

	public void setAllowedErrorUsers(float f) {
		AllowedErrorForSyncedPlaybackUsers = f;
	}

	public void setMaxErrorUsers(float f) {
		MaximumErrorForSyncedPlaybackUsers = f;
	}

	public void setTimeDilationUsers(float f) {
		SyncedPlaybackMaximumTimeDilationUsers = f;
	}

}
