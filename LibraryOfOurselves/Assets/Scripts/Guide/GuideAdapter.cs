
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GuideAdapter : MonoBehaviour{

	[SerializeField] UnityEvent onZeroConnections;
	[SerializeField] UnityEvent onFirstConnection;

	public static GuideAdapter Instance { get; private set; }

	private void Start() {
		Instance = this;
		onZeroConnections.Invoke();
	}

	private void OnDestroy() {
		Instance = null;
	}


	public void OnNewConnection(TCPConnection connection) {
		Debug.Log(connection + " has been added as an option");
		if(ConnectionsDisplayer.Instance) {
			ConnectionsDisplayer.Instance.AddConnection(connection);
			if(ConnectionsDisplayer.Instance.Handles.Count == 1)
				onFirstConnection.Invoke();
		}
	}

	public void OnConnectionEnd(TCPConnection connection) {
		connection.active = false;
		connection.paired = false;
		Debug.Log(connection + " is no longer available");
		if(ConnectionsDisplayer.Instance) {
			ConnectionsDisplayer.Instance.RemoveConnection(connection);
			if(ConnectionsDisplayer.Instance.Handles.Count <= 0) {
				onZeroConnections.Invoke();
			}
		}
	}

	public void OnConnectionResponsivenessChanged(TCPConnection connection, bool isResponsive) {
		ConnectionsDisplayer.DisplayedConnectionHandle handle = null;
		if(ConnectionsDisplayer.Instance) {
			handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
			if(handle != null)
				handle.display.UpdateDisplay();
		}
	}

	public void SendGuideLock(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-lock");
		connection.Send(data);
	}

	public void SendGuideUnlock(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-unlock");
		connection.Send(data);
	}

	public void OnReceiveAutopair(TCPConnection connection) {
		Debug.Log(connection.ToString() + " requests autopair.");
		SendGuidePair(connection);
	}

	public void SendGuidePair(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-pair");
		connection.Send(data);
	}

	public void SendGuideUnpair(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-unpair");
		OnReceivePairConfirm(connection, "0", connection.lockedId);//simulate receiving an immediate response when unpairing.
		connection.Send(data);
	}

	public void OnReceivePairConfirm(TCPConnection connection, string pairedId, string lockedId) {

		ConnectionsDisplayer.DisplayedConnectionHandle handle = null;
		if(ConnectionsDisplayer.Instance) {
			handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
		}

		bool paired = pairedId != "0";//string "0" means the device is unpaired now
		if(pairedId == SystemInfo.deviceUniqueIdentifier) {
			Debug.Log("Paired to " + connection);
			connection.paired = true;
			if(handle != null) handle.display.Available = true;
			//Give an update to the VideosDisplayer
			if(VideosDisplayer.Instance) {
				VideosDisplayer.Instance.OnPairConnection(connection);
			}
		} else {
			if(connection.paired) {
				connection.paired = false;
				Debug.Log("Unpaired from " + connection);
			}
			if(handle != null) handle.display.Available = !paired;
		}
		
		connection.lockedId = lockedId;

		if(handle != null) {
			handle.display.UpdateDisplay();
		}
		
	}

	public void SendLogsQuery(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("logs-query");
		connection.Send(data);
	}

	public void OnReceiveLogs(TCPConnection connection, string log) {
		Debug.Log("Received logs from "+connection+":\n" + log);
	}

	public void SendHasVideo(TCPConnection connection, string videoName) {
		List<byte> data = new List<byte>();
		data.WriteString("has-video");
		data.WriteString(videoName);
		//if(TCPHost.Instance)
		//	TCPHost.Instance.BroadcastToPairedDevices(data);
		connection.Send(data);
	}

	public void OnReceiveHasVideoResponse(TCPConnection connection, string videoName) {
		Debug.Log(connection + " has video " + videoName);
		ConnectionsDisplayer.DisplayedConnectionHandle handle = null;
		if(ConnectionsDisplayer.Instance) {
			handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
			if(handle != null) {
				handle.display.AddAvailableVideo(videoName);
			}
		}
	}

	public void OnReceiveIsEmpty(TCPConnection connection) {
		//Debug.Log(connection + " is empty");
		// should we take note?
	}

	public void SendLoadVideo(string videoName, string mode) {
		List<byte> data = new List<byte>();
		data.WriteString("load-video");
		data.WriteString(videoName);
		data.WriteString(mode);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
		if(ConnectionsDisplayer.Instance) {
			foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
				handle.display.IsVideoReady = false;
			}
		}
	}

	public void OnReceiveLoadVideoResponse(TCPConnection connection, bool ok, string errorMessage) {
		if(ok) {
			Debug.Log(connection + " has successfully loaded video.");
			ConnectionsDisplayer.DisplayedConnectionHandle handle = null;
			if(ConnectionsDisplayer.Instance) {
				handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
				if(handle != null)
					handle.display.IsVideoReady = true;
			}
		} else {
			Debug.LogWarning("Error: " + connection + " could not load video: " + errorMessage);
			//TODO: display error message
		}
	}

	public void SendPlayVideo() {
		List<byte> data = new List<byte>();
		data.WriteString("play-video");
		data.WriteTimestamp(DateTime.Now);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendPauseVideo(double videoTime, bool pause) {
		List<byte> data = new List<byte>();
		data.WriteString("pause-video");
		data.WriteTimestamp(DateTime.Now);
		data.WriteDouble(videoTime);
		data.WriteBool(pause);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendStopVideo() {
		List<byte> data = new List<byte>();
		data.WriteString("stop-video");
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendSync(double videoTime) {
		List<byte> data = new List<byte>();
		data.WriteString("sync");
		data.WriteTimestamp(DateTime.Now);
		data.WriteDouble(videoTime);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendCalibrate(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("calibrate");
		connection.Send(data);
	}

	public void SendStartChoice(string question, string choice1, string choice2) {
		List<byte> data = new List<byte>();
		data.WriteString("start-choice");
		data.WriteString(question);
		data.WriteString(choice1);
		data.WriteString(choice2);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void OnReceiveSelectOption(TCPConnection connection, byte option) {
		if(GuideVideoPlayer.Instance != null)
			GuideVideoPlayer.Instance.OnReceiveChoiceConfirmation(connection, (int)option - 1);
	}

	public void SendReorient(Vector3 angles) {
		List<byte> data = new List<byte>();
		data.WriteString("reorient");
		data.WriteVector3(angles);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void OnReceiveStatus(TCPConnection connection, byte b_battery, short s_fps, byte b_temp) {
		int battery = (int)b_battery;
		float fps = ((float)s_fps) * 0.1f;
		int temperature = (int)b_temp;
		if(ConnectionsDisplayer.Instance) {
			ConnectionsDisplayer.DisplayedConnectionHandle handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
			if(handle != null) {
				handle.display.Battery = battery;
				handle.display.FPS = fps;
				handle.display.Temperature = temperature;
			} else {
				Debug.LogWarning("Received status from a non-existent connection, apparently...");
			}
		}
	}

}
