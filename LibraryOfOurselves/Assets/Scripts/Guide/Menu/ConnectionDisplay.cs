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
			temperatureDisplay.text = value + "°";
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
		pairDisplay.text = Connection.paired ? "Paired" : "Unpaired";
		lockDisplay.text = Connection.lockedId == SystemInfo.deviceUniqueIdentifier ? "Locked to this one" : Connection.lockedId == "free" ? "Free" : "Locked to something else";
	}

}
