
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class ConnectionEvent : UnityEvent<TCPConnection> { }

public class GuideAdapter : MonoBehaviour{

	[SerializeField] UnityEvent onZeroConnections;
	[SerializeField] UnityEvent onFirstConnection;

	public delegate void OnReceivedChoicePosition(Vector3 eulerAngles);
	public static OnReceivedChoicePosition ReceivedChoicePosition;

	public static GuideAdapter Instance { get; private set; }

	private void OnEnable()
	{
		ChoiceOption.EditButtonClicked += SendEditChoice;
		ChoiceOption.SaveButtonClicked += SendSaveChoice;
	}

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

	public void OnReceivePairConfirm(TCPConnection connection, string pairedId, string lockedId) { //

		ConnectionsDisplayer.DisplayedConnectionHandle handle = null;
		if (ConnectionsDisplayer.Instance) {
			handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
		}

		bool paired = pairedId != "0";//string "0" means the device is unpaired now
		if (pairedId == SystemInfo.deviceUniqueIdentifier) { //check if this is a device we were paired to
			Debug.Log("Paired to " + connection);
			connection.paired = true;
			if(handle != null) handle.display.Available = true;
			//Give an update to the VideosDisplayer
			if(VideosDisplayer.Instance) {
				VideosDisplayer.Instance.OnPairConnection(connection);
			}
		} else {
			if (connection.paired) {
				connection.paired = false;
				Debug.Log("Unpaired from " + connection);
			}
			Debug.LogWarning("Setting available to " + !paired + " for connection " + connection);
			if (handle != null) handle.display.Available = !paired;
		}
		
		connection.lockedId = lockedId;

		//Update all device displays
		if (ConnectionsDisplayer.Instance) {
			foreach (ConnectionsDisplayer.DisplayedConnectionHandle h in ConnectionsDisplayer.Instance.Handles) {
				h.display.UpdateDisplay();
			}
		}

	}

	public void SendLogsQueryAll() {
		List<byte> data = new List<byte>();
		data.WriteString("logs-query");
		Debug.Log("Requesting logs...");
		TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendLogsQuery(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("logs-query");
		connection.Send(data);
	}

	public void OnReceiveLogs(TCPConnection connection, string log) {
		string path = "[Unknown path]";
		try {
			Debug.Log("Received logs from " + connection.ToString() + ".");
			Debug.Log("Log contents: [[[[[\n" + log + "\n]]]]]");

			//Generate two random words for saving the file
			path = "loo_log_" + RandomWords.Noun + "_" + RandomWords.Noun + ".txt";

			//Write to file
			string fullPath = Application.persistentDataPath;
			if(fullPath[fullPath.Length - 1] != '/' && fullPath[fullPath.Length - 1] != '\\') {
				fullPath += "/";
			}
			//fullPath += path;
			string rf = "Logs for device " + connection.ToString() + " at " + DateTime.Now.ToString() + ":\n" + log;
			rf.Replace("\n", "\r\n");
			//File.WriteAllText(fullPath, rf);
			FileWriter.WriteFile(fullPath, path, rf);

			Debug.Log("Wrote log to file " + path);
			Debug.Log("(Full path: " + fullPath + ")");
		}catch(Exception e) {
			Debug.LogError("Error: Could not write to file " + path + ": " + e.Message);
			Debug.LogError(e.StackTrace);
		}
	}

	public void SendHasVideo(TCPConnection connection, string videoName) {
		List<byte> data = new List<byte>();
		data.WriteString("has-video");
		data.WriteString(videoName);
		//if(TCPHost.Instance)
		//	TCPHost.Instance.BroadcastToPairedDevices(data);
		connection.Send(data);
		Debug.Log("Requesting devices for video " + videoName + "..");
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
		data.WriteFloat(Settings.SyncTime);
		data.WriteVector3(new Vector3(
			Settings.AllowedErrorForSyncedPlaybackUsers,
			Settings.MaximumErrorForSyncedPlaybackUsers,
			Settings.SyncedPlaybackMaximumTimeDilationUsers
		));
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

	public void OnReceiveSync(TCPConnection connection, DateTime timestamp, double videoTime) {
		if(!Settings.SendSyncMessages) return;//Ignore this packet if Sync is disabled.
		//only take it into account if it's the only device we're paired with
		if(connection != null && connection.paired) {
			int pairedDevices = 0;
			if(ConnectionsDisplayer.Instance) {
				foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
					if(handle.connection.active && handle.connection.paired) {
						++pairedDevices;
					}
				}
			}
			if(pairedDevices == 1 && !Settings.ForceMultiUserSetup) {
				//ok, we can pass that on to the video player.
				GuideVideoPlayer.Instance.Sync(timestamp, videoTime);
			}
		}
	}

	public void SendCalibrate(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("calibrate");
		connection.Send(data);
	}

	public void SendStartChoice(string question, List<VideoChoice> options) {
		List<byte> data = new List<byte>();
		data.WriteString("start-choice");
		string optionsDescriptions = ""; 
		string optionsPositions = ""; 
		for(int i = 0; i < options.Count; i++)
		{
			var option = options[i];
			optionsDescriptions += option.description;
			if (i < options.Count - 1) optionsDescriptions += "," ;
			optionsPositions += option.position.x + ",";
			optionsPositions += option.position.y + ",";
			optionsPositions += option.position.z;
			if (i < options.Count - 1) optionsPositions += "#";
		}
		data.WriteString(question);
		data.WriteString(optionsDescriptions);
		data.WriteString(optionsPositions);
		if (TCPHost.Instance) TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendEditChoice(string videoName, string description, Vector3 eulerAngles) //TODO must include start video and selected video
	{
		List<byte> data = new List<byte>();
		data.WriteString("edit-choice");
		data.WriteString(VideoDisplay.expandedDisplay.VideoName); //start video
		//TODO add end video name data.writestring(endVideoName)
		data.WriteString(description);
		data.WriteString(eulerAngles.x.ToString());
		data.WriteString(eulerAngles.y.ToString());
		data.WriteString(eulerAngles.z.ToString());
		if (TCPHost.Instance) TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendSaveChoice(string videoName, string description, Vector3 eulerAngles)
	{
		List<byte> data = new List<byte>();
		data.WriteString("save-choice");
		data.WriteString(videoName);
		data.WriteString(description);
		data.WriteString(eulerAngles.x.ToString());
		data.WriteString(eulerAngles.y.ToString());
		data.WriteString(eulerAngles.z.ToString());
		if (TCPHost.Instance) TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void OnReceiveChoicePosition(TCPConnection connection, string eulerAngles)
	{
		Debug.Log("received choice position " + eulerAngles);
		Vector3 eulerAnglesVector3 = new Vector3(
			 float.Parse(eulerAngles.Split(',')[0]),
			 float.Parse(eulerAngles.Split(',')[1]),
			 float.Parse(eulerAngles.Split(',')[2])
		);
		ReceivedChoicePosition.Invoke(eulerAnglesVector3);
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

	//these two properties exclusively for use within 0ld guide
	public static float LastFPSReceived { get; private set; }
	public static int LastBatteryReceived { get; private set; }

	public void OnReceiveStatus(TCPConnection connection, byte b_battery, short s_fps, byte b_temp) {
		int battery = (int)b_battery;
		float fps = ((float)s_fps) * 0.1f;
		int temperature = (int)b_temp;

		LastFPSReceived = fps;
		LastBatteryReceived = battery;

		if(ConnectionsDisplayer.Instance) {
			ConnectionsDisplayer.DisplayedConnectionHandle handle = ConnectionsDisplayer.Instance.GetConnectionHandle(connection);
			if(handle != null) {
				handle.display.Battery = battery;
				handle.display.FPS = fps;
			} else {
				Debug.LogWarning("Received status from a non-existent connection, apparently...");
			}
		}
	}

	public void SendGotoTime(double time) {
		//Broadcast a time to all user devices
		List<byte> data = new List<byte>();
		data.WriteString("goto");
		data.WriteDouble(time);
		TCPHost.Instance.BroadcastToPairedDevices(data);
	}

}
