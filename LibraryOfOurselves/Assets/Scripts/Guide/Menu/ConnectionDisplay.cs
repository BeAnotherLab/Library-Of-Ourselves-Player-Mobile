﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDisplay : MonoBehaviour
{
	[SerializeField] private Text _modelNameDisplay;
	[SerializeField] private Text _batteryDisplay;
	[SerializeField] private Text _fpsDisplay;
	[SerializeField] private Image _uniqueIdColourDisplay;
	[SerializeField] private Image _statusDisplay;
	[SerializeField] private Color _pairedColour;
	[SerializeField] private Color _availableColour;
	[SerializeField] private Color _unavailableColour;
	[SerializeField] private Interpolation _lockDisplay;
	[SerializeField] private Text _textPair;
	[SerializeField] private Text _textUnpair;
	[SerializeField] private Button _pairButton;
	[SerializeField] private Text _textLock;
	[SerializeField] private Text _textUnlock;
	[SerializeField] private Button _lockButton;
	[SerializeField] private ColorDynamicModifier _lockColourModifier;
	[SerializeField] private Color _lockAvailableColour;
	[SerializeField] private Color _lockUnavailableColour;
	[SerializeField] private Button _recenterButton;
	[SerializeField] private Button _editDeviceNameButton;
	[SerializeField] private InputField _editDeviceNameField;
	[SerializeField] private GameObject _udpDisplay;

	public TCPConnection Connection { get; private set; }

	public int Battery {
		set {
			_batteryDisplay.text = value + "%";
		}
	}

	public float FPS {
		set {
			_fpsDisplay.text = value + " FPS";
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
			Debug.LogError("Cannot reinit a ConnectionDisplay!");
			return;
		}
		_initialized = true;
		Connection = connection;
		UpdateDisplay();
		_uniqueIdColourDisplay.color = DeviceColour.getDeviceColor(connection.uniqueId);
		_modelNameDisplay.text = DeviceAlias;

		_udpDisplay.SetActive(connection.UDP);

		_editDeviceNameField.gameObject.SetActive(false);
	}

	public void AddAvailableVideo(string videoName) {
		Debug.Log("Adding video " + videoName + " to device " + Connection);
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
				Debug.Log("Sending pair");
				GuideAdapter.Instance.SendGuidePair(Connection);
			} 
			else {
				Debug.Log("Sending unpair");
				GuideAdapter.Instance.SendGuideUnpair(Connection);
			}
		}
	}

	public void UpdateDisplay() {

		if (Connection.paired) {
			_statusDisplay.color = _pairedColour;
			_textPair.gameObject.SetActive(false);
			_textUnpair.gameObject.SetActive(true);
			_pairButton.gameObject.SetActive(true);
			_lockButton.gameObject.SetActive(true);
			_recenterButton.gameObject.SetActive(true);
		} 
		else if (Available) {
			_statusDisplay.color = _availableColour;
			_textPair.gameObject.SetActive(true);
			_textUnpair.gameObject.SetActive(false);
			_lockButton.gameObject.SetActive(false);
			_recenterButton.gameObject.SetActive(false);
			
			//TODO what do we do when losing connection during playback
			if (GuideVideoPlayer.Instance.HasVideoLoaded) {//can't pair with a device while a video is loaded up. 
				_pairButton.gameObject.SetActive(false);
			}
			else {
				if (SettingsAuth.TemporalUnlock) {
					_pairButton.gameObject.SetActive(true);
					Debug.Log("Displaying PAIR button for connection " + Connection + " because this guide device has temporal unlock.");
				}
				else {
					int pairedDevices = 0;
					foreach (ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
						if (handle.connection.paired) ++pairedDevices;
					}
					_pairButton.gameObject.SetActive(pairedDevices <= 0); //Only show if we're not connected to a device yet
				}
			}
			StartCoroutine(EnableUnlockButtonAfterABit());
		} 
		else {
			Debug.Log("Connection " + Connection + " is not available. Hiding LOCK and PAIR buttons.");
			_statusDisplay.color = _unavailableColour;
			_pairButton.gameObject.SetActive(false);
			_lockButton.gameObject.SetActive(false);
			_recenterButton.gameObject.SetActive(false);
			StartCoroutine(EnableUnlockButtonAfterABit());
		}

		//the lock is closed and the device is unlocked:
		if (_hasClosedLock && Connection.lockedId == "free") {
			_hasClosedLock = false;
			_textLock.gameObject.SetActive(true);
			_textUnlock.gameObject.SetActive(false);
			_lockDisplay.InterpolateBackward();//open lock
			_lockColourModifier.DefaultColor = _lockUnavailableColour;
		} 
		else if (!_hasClosedLock && Connection.lockedId != "free") {
			_hasClosedLock = true;
			_textLock.gameObject.SetActive(false);
			_textUnlock.gameObject.SetActive(true);
			_lockDisplay.Interpolate(); //close lock
			if (Connection.lockedId == SystemInfo.deviceUniqueIdentifier) { //lock color
				_lockColourModifier.DefaultColor = _lockAvailableColour; //green
			} else {
				_lockColourModifier.DefaultColor = _lockUnavailableColour; //grey
			}
		}

		if (!SettingsAuth.TemporalUnlock) { //If we don't have Admin Access, disable lock button and edit device name abilities.
			_editDeviceNameButton.enabled = false;
			_lockButton.gameObject.SetActive(false);
			_editDeviceNameButton.enabled = true;
		}
	}
	
	public void OnClickEditDeviceName() {
		_editDeviceNameField.gameObject.SetActive(true);
		_editDeviceNameField.text = DeviceAlias;
	}

	public void OnSubmitNewDeviceName() {
		_editDeviceNameField.gameObject.SetActive(false);
		DeviceAlias = _editDeviceNameField.text;
		_modelNameDisplay.text = DeviceAlias;
	}

	public void OnClickCloseButton() {
		Connection.active = false;
	}

	private IEnumerator EnableUnlockButtonAfterABit() { //TODO why do we have to wait before showing unlock button?
		yield return new WaitForSeconds(3);
		if (Connection.lockedId != "free && !Connection.paired")  _lockButton.gameObject.SetActive(true); //are we allowed to show the unlock button?
	}
}
