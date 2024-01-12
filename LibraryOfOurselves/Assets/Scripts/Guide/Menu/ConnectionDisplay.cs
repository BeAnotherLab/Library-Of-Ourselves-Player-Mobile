using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDisplay : MonoBehaviour
{
	[SerializeField] private Text modelNameDisplay;
	[SerializeField] private Text batteryDisplay;
	[SerializeField] private Text fpsDisplay;
	[SerializeField] private Image uniqueIdColourDisplay;
	[SerializeField] private Image statusDisplay;
	[SerializeField] private Color pairedColour;
	[SerializeField] private Color availableColour;
	[SerializeField] private Color unavailableColour;
	[SerializeField] private Interpolation lockDisplay;
	[SerializeField] private Text textPair;
	[SerializeField] private Text textUnpair;
	[SerializeField] private Button pairButton;
	[SerializeField] private Text textLock;
	[SerializeField] private Text textUnlock;
	[SerializeField] private Button lockButton;
	[SerializeField] private ColorDynamicModifier lockColourModifier;
	[SerializeField] private Color lockAvailableColour;
	[SerializeField] private Color lockUnavailableColour;
	[SerializeField] private Button recenterButton;
	[SerializeField] private Button editDeviceNameButton;
	[SerializeField] private InputField editDeviceNameField;
	[SerializeField] private GameObject udpDisplay;

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

	public bool IsVideoReady { get; set; }
	
	public bool Available {//True when it's possible for us to connect to this device
		get {
			if (Connection == null || Connection.active == false)
				return false; //this device is not responding anymore... (this means we'll be getting rid of this display soon anyways)
			if (Connection.paired)
				return true; //in any case we're connected so...
			return __available && (Connection.lockedId == SystemInfo.deviceUniqueIdentifier || Connection.lockedId == "free");
		}
		set {
			__available = value;
		}
	}

	public List<string> VideosAvailable { get { return __videosAvailable; } }

	private string DeviceAlias {
		get {
			string alias = "";
			if (HazePrefs.HasKey("alias-" + Connection.uniqueId)) {
				alias = HazePrefs.GetString("alias-" + Connection.uniqueId);
			}
			if (alias == "")
				return Connection.xrDeviceModel;
			else
				return alias;
		}
		set {
			if (value == "" || value == Connection.xrDeviceModel) {
				if (HazePrefs.HasKey("alias-" + Connection.uniqueId)) {
					HazePrefs.DeleteKey("alias-" + Connection.uniqueId);
				}
			} else {
				HazePrefs.SetString("alias-" + Connection.uniqueId, value);
			}
		}
	}
	
	private List<string> __videosAvailable = new List<string>();

	private bool _hasClosedLock;

	private bool __available = true;

	private bool _initialized;
	
	public void Init(TCPConnection connection) {
		if (_initialized) {
			Haze.Logger.LogError("Cannot reinit a ConnectionDisplay!");
			return;
		}
		_initialized = true;
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

	public void OnClickCalibrate() { //recenter
		if (GuideAdapter.Instance)
			GuideAdapter.Instance.SendCalibrate(Connection);
	}

	public void OnClickLock() {
		if (GuideAdapter.Instance) 
		{
			if (Connection.lockedId == "free") 
			{
				GuideAdapter.Instance.SendGuideLock(Connection); //lock it to us
			} 
			else 
			{
				//unlock it
				if (Connection.lockedId != SystemInfo.deviceUniqueIdentifier) {
					GuideAdapter.Instance.SendGuideUnpair(Connection); //if the connection was locked with another guide, first request that it unpairs. This is quite unsafe actually and will result in a warning device-side :) Just to keep in mind.
				}
				GuideAdapter.Instance.SendGuideUnlock(Connection);
			}
		}
	}

	public void OnClickPair() {
		if (GuideAdapter.Instance) 
		{
			if (!Connection.paired) {
				Haze.Logger.Log("Sending pair");
				GuideAdapter.Instance.SendGuidePair(Connection);
			} 
			else {
				Haze.Logger.Log("Sending unpair");
				GuideAdapter.Instance.SendGuideUnpair(Connection);
			}
		}
	}

	public void UpdateDisplay() {

		if (Connection.paired) {
			statusDisplay.color = pairedColour;
			textPair.gameObject.SetActive(false);
			textUnpair.gameObject.SetActive(true);
			pairButton.gameObject.SetActive(true);
			lockButton.gameObject.SetActive(true);
			recenterButton.gameObject.SetActive(true);
		} 
		else if (Available) {
			statusDisplay.color = availableColour;
			textPair.gameObject.SetActive(true);
			textUnpair.gameObject.SetActive(false);
			lockButton.gameObject.SetActive(false);
			recenterButton.gameObject.SetActive(false);
			
			//TODO what do we do when losing connection during playback
			if (GuideVideoPlayer.Instance.HasVideoLoaded) {//can't pair with a device while a video is loaded up. 
				pairButton.gameObject.SetActive(false);
			}
			else {
				if (SettingsAuth.TemporalUnlock) {
					pairButton.gameObject.SetActive(true);
					Haze.Logger.Log("Displaying PAIR button for connection " + Connection + " because this guide device has temporal unlock.");
				}
				else {
					int pairedDevices = 0;
					foreach (ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
						if (handle.connection.paired) ++pairedDevices;
					}
					pairButton.gameObject.SetActive(pairedDevices <= 0); //Only show if we're not connected to a device yet
				}
			}
			StartCoroutine(EnableUnlockButtonAfterABit());
		} 
		else {
			Haze.Logger.Log("Connection " + Connection + " is not available. Hiding LOCK and PAIR buttons.");
			statusDisplay.color = unavailableColour;
			pairButton.gameObject.SetActive(false);
			lockButton.gameObject.SetActive(false);
			recenterButton.gameObject.SetActive(false);
			StartCoroutine(EnableUnlockButtonAfterABit());
		}

		//the lock is closed and the device is unlocked:
		if (_hasClosedLock && Connection.lockedId == "free") {
			_hasClosedLock = false;
			textLock.gameObject.SetActive(true);
			textUnlock.gameObject.SetActive(false);
			lockDisplay.InterpolateBackward();//open lock
			lockColourModifier.DefaultColor = lockUnavailableColour;
		} 
		else if (!_hasClosedLock && Connection.lockedId != "free") {
			_hasClosedLock = true;
			textLock.gameObject.SetActive(false);
			textUnlock.gameObject.SetActive(true);
			lockDisplay.Interpolate(); //close lock
			if (Connection.lockedId == SystemInfo.deviceUniqueIdentifier) { //lock color
				lockColourModifier.DefaultColor = lockAvailableColour; //green
			} else {
				lockColourModifier.DefaultColor = lockUnavailableColour; //grey
			}
		}

		if (!SettingsAuth.TemporalUnlock) { //If we don't have Admin Access, disable lock button and edit device name abilities.
			editDeviceNameButton.enabled = false;
			lockButton.gameObject.SetActive(false);
			editDeviceNameButton.enabled = true;
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

	private IEnumerator EnableUnlockButtonAfterABit() { //TODO why do we have to wait before showing unlock button?
		yield return new WaitForSeconds(3);
		if (Connection.lockedId != "free && !Connection.paired")  lockButton.gameObject.SetActive(true); //are we allowed to show the unlock button?
	}
}
