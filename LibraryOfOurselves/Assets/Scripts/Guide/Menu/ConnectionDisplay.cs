using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDisplay : MonoBehaviour{

	[SerializeField] Text batteryDisplay;
	[SerializeField] Text fpsDisplay;
	[SerializeField] Text temperatureDisplay;
	[SerializeField] Text pairDisplay;
	[SerializeField] Text lockDisplay;
	[SerializeField] Image uniqueIdColourDisplay;

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

	public int Temperature {
		set {
			if(value == int.MaxValue) {
				//unavailable
				temperatureDisplay.text = "No temp";
			} else {
				temperatureDisplay.text = value + "°";
			}
		}
	}

	List<string> __videosAvailable = new List<string>();
	public List<string> VideosAvailable { get { return __videosAvailable; } }

	public bool IsVideoReady { get; set; }


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
			Debug.LogError("Cannot reinit a ConnectionDisplay!");
			return;
		}
		initialized = true;
		Connection = connection;
		UpdateDisplay();
		uniqueIdColourDisplay.color = DeviceColour.getDeviceColor(connection.uniqueId);
	}

	public void AddAvailableVideo(string videoName) {
		VideosAvailable.Add(videoName);
	}

	public void OnClickCalibrate() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendCalibrate(Connection);
	}

	public void OnClickLock() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendGuideLock(Connection);
	}

	public void OnClickUnlock() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendGuideUnlock(Connection);
	}

	public void OnClickPair() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendGuidePair(Connection);
	}

	public void OnClickUnpair() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendGuideUnpair(Connection);
	}

	public void OnClickLogs() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendLogsQuery(Connection);
	}

	public void UpdateDisplay() {
		pairDisplay.text = Connection.paired ? "Paired" : "Unpaired (Available: " + Available + ")";
		lockDisplay.text = Connection.lockedId == SystemInfo.deviceUniqueIdentifier ? "Locked to this one" : Connection.lockedId == "free" ? "Free" : "Locked to something else";
		pairDisplay.color = Connection.responsive ? new Color(0, 0, 0) : new Color(0.5f, 0.5f, 0.5f);
		lockDisplay.color = pairDisplay.color;
	}

}
