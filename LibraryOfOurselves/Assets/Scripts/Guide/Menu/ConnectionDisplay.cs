using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDisplay : MonoBehaviour{

	[SerializeField] Text modelNameDisplay;
	[SerializeField] Text batteryDisplay;
	[SerializeField] Text fpsDisplay;
	[SerializeField] Image uniqueIdColourDisplay;
	[SerializeField] Image statusDisplay;
	[SerializeField] Color pairedColour;
	[SerializeField] Color availableColour;
	[SerializeField] Color unavailableColour;
	[SerializeField] Interpolation lockDisplay;
	[SerializeField] Text textPair;
	[SerializeField] Text textUnpair;
	[SerializeField] Button pairButton;
	[SerializeField] Text textLock;
	[SerializeField] Text textUnlock;
	[SerializeField] Button lockButton;
	[SerializeField] ColorDynamicModifier lockColourModifier;
	[SerializeField] Color lockAvailableColour;
	[SerializeField] Color lockUnavailableColour;
	[SerializeField] Button recenterButton;
	[SerializeField] Button editDeviceNameButton;
	[SerializeField] InputField editDeviceNameField;
	[SerializeField] GameObject udpDisplay;

	public TCPConnection Connection { get; private set; }

	public int Battery {
		set {
			batteryDisplay.text = value + "%";
		}
	}

	public float FPS {
		set {
			fpsDisplay.text = value + " FPS";
		}
	}

	string DeviceAlias {
		get {
			string alias = "";
			if(HazePrefs.HasKey("alias-" + Connection.uniqueId)) {
				alias = HazePrefs.GetString("alias-" + Connection.uniqueId);
			}
			if(alias == "")
				return Connection.xrDeviceModel;
			else
				return alias;
		}
		set {
			if(value == "" || value == Connection.xrDeviceModel) {
				if(HazePrefs.HasKey("alias-" + Connection.uniqueId)) {
					HazePrefs.DeleteKey("alias-" + Connection.uniqueId);
				}
			} else {
				HazePrefs.SetString("alias-" + Connection.uniqueId, value);
			}
		}
	}

	List<string> __videosAvailable = new List<string>();
	public List<string> VideosAvailable { get { return __videosAvailable; } }

	public bool IsVideoReady { get; set; }

	bool hasClosedLock = false;

	bool __available = true;
	public bool Available {//True when it's possible for us to connect to this device
		get {
			if(Connection == null || Connection.active == false)
				return false;//this device is not responding anymore... (this means we'll be getting rid of this display soon anyways)
			if(Connection.paired)
				return true;//in any case we're connected so...
			return __available && (Connection.lockedId == SystemInfo.deviceUniqueIdentifier || Connection.lockedId == "free");
		}
		set {
			__available = value;
		}
	}

	bool initialized = false;
	public void Init(TCPConnection connection) {
		if(initialized) {
			Haze.Logger.LogError("Cannot reinit a ConnectionDisplay!");
			return;
		}
		initialized = true;
		Connection = connection;
		UpdateDisplay();
		uniqueIdColourDisplay.color = DeviceColour.getDeviceColor(connection.uniqueId);
		modelNameDisplay.text = DeviceAlias;

		udpDisplay.SetActive(connection.UDP);

		editDeviceNameField.gameObject.SetActive(false);
	}

	public void AddAvailableVideo(string videoName) {
		Haze.Logger.Log("Adding video " + videoName + " to device " + Connection);
		VideosAvailable.Add(videoName);
	}

	public void OnClickCalibrate() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendCalibrate(Connection);
	}

	public void OnClickLock() {
		if(GuideAdapter.Instance) {
			if(Connection.lockedId == "free") {
				//lock it to us
				GuideAdapter.Instance.SendGuideLock(Connection);
			} else {
				//unlock it
				if(Connection.lockedId != SystemInfo.deviceUniqueIdentifier) {
					GuideAdapter.Instance.SendGuideUnpair(Connection);//if the connection was locked with another guide, first request that it unpairs. This is quite unsafe actually and will result in a warning device-side :) Just to keep in mind.
				}
				GuideAdapter.Instance.SendGuideUnlock(Connection);
			}
		}
	}

	public void OnClickPair() {
		if(GuideAdapter.Instance) {
			if(!Connection.paired) {
				Haze.Logger.Log("Sending pair");
				GuideAdapter.Instance.SendGuidePair(Connection);
			} else {
				Haze.Logger.Log("Sending unpair");
				GuideAdapter.Instance.SendGuideUnpair(Connection);
			}
		}
	}

	public void OnClickLogs() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendLogsQuery(Connection);
	}

	public void UpdateDisplay() {

		Color statusColor = statusDisplay.color;
		Color uniqueIdColor = uniqueIdColourDisplay.color;

		if(Connection.paired) {
			Haze.Logger.Log("Connection " + Connection + " is paired. Showing LOCK and PAIR buttons by default.");
			statusColor = pairedColour;
			textPair.gameObject.SetActive(false);
			textUnpair.gameObject.SetActive(true);
			pairButton.gameObject.SetActive(true);
			lockButton.gameObject.SetActive(true);
			recenterButton.gameObject.SetActive(true);
			
		} else if(Available) {
			Haze.Logger.Log("Connection " + Connection + " is available.");
			statusColor = availableColour;
			textPair.gameObject.SetActive(true);
			textUnpair.gameObject.SetActive(false);
			lockButton.gameObject.SetActive(false);
			Haze.Logger.Log("Hiding LOCK button for connection " + Connection + " because we aren't currently paired to it - will become visible after a bit.");
			recenterButton.gameObject.SetActive(false);
			if(GuideVideoPlayer.Instance.HasVideoLoaded) {//can't pair with a device while a video is loaded up.
				pairButton.gameObject.SetActive(false);
				Haze.Logger.Log("Hiding PAIR button for connection " + Connection + " because a video is loaded at the moment.");
			} else {
				if(SettingsAuth.TemporalUnlock) {
					pairButton.gameObject.SetActive(true);
					Haze.Logger.Log("Displaying PAIR button for connection " + Connection + " because this guide device has temporal unlock.");
				} else {
					int pairedDevices = 0;
					foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
						if(handle.connection.paired)
							++pairedDevices;
					}
					pairButton.gameObject.SetActive(pairedDevices <= 0);//Only show if we're not connected to a device yet
					if(pairedDevices <= 0) Haze.Logger.Log("Displaying PAIR button for connection " + Connection + " because this guide is not connected to any other.");
					else Haze.Logger.Log("Hiding PAIR button for connection " + Connection + " because this guide is connected to other devices and is not allowed to enter Multi-user mode.");
				}
			}
			StartCoroutine(enableUnlockButtonAfterABit());
		} else {
			Haze.Logger.Log("Connection " + Connection + " is not available. Hiding LOCK and PAIR buttons.");
			statusColor = unavailableColour;
			pairButton.gameObject.SetActive(false);
			lockButton.gameObject.SetActive(false);
			recenterButton.gameObject.SetActive(false);
			StartCoroutine(enableUnlockButtonAfterABit());
		}

		if(Connection.responsive) {
			statusColor.a = 1;
			uniqueIdColor.a = 1;
		} else {
			statusColor.a = 0.3f;
			uniqueIdColor.a = 0.3f;
		}

		statusDisplay.color = statusColor;
		uniqueIdColourDisplay.color = uniqueIdColor;

		//the lock is closed and the device is unlocked:
		if(hasClosedLock && Connection.lockedId == "free") {
			hasClosedLock = false;
			textLock.gameObject.SetActive(true);
			textUnlock.gameObject.SetActive(false);
			lockDisplay.InterpolateBackward();//open lock
			lockColourModifier.DefaultColor = lockUnavailableColour;
		}else if(!hasClosedLock && Connection.lockedId != "free") {
			hasClosedLock = true;
			textLock.gameObject.SetActive(false);
			textUnlock.gameObject.SetActive(true);
			lockDisplay.Interpolate();//close lock
			//Should the lock be green or grey?
			if(Connection.lockedId == SystemInfo.deviceUniqueIdentifier) {
				//green lock
				lockColourModifier.DefaultColor = lockAvailableColour;
			} else {
				//grey lock
				lockColourModifier.DefaultColor = lockUnavailableColour;
			}
		}

		//If we don't have Admin Access, disable lock button and edit device name abilities.
		if(!SettingsAuth.TemporalUnlock) {
			editDeviceNameButton.enabled = false;
			lockButton.gameObject.SetActive(false);
			editDeviceNameButton.enabled = true;
		}
	}

	IEnumerator enableUnlockButtonAfterABit() {
		yield return new WaitForSeconds(3);
		if(!Connection.paired) {
			//are we allowed to show the unlock button?
			if(Connection.lockedId != "free") {
				lockButton.gameObject.SetActive(true);
				Haze.Logger.Log("Showing LOCK button because connection " + Connection + " is locked to " + Connection.lockedId + " and we've waited 3 seconds.");
			} else {
				Haze.Logger.Log("Not showing LOCK button because connection " + Connection + " is not locked.");
			}
		}
	}

	public void OnClickEditDeviceName() {
		editDeviceNameField.gameObject.SetActive(true);
		editDeviceNameField.text = DeviceAlias;
	}

	public void OnSubmitNewDeviceName() {
		editDeviceNameField.gameObject.SetActive(false);
		DeviceAlias = editDeviceNameField.text;
		modelNameDisplay.text = DeviceAlias;
	}

	public void OnClickCloseButton() {
		Connection.active = false;
	}

}
