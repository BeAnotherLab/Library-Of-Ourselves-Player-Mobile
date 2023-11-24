using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class VRAdapter : MonoBehaviour{

	[SerializeField] float sendStatusEvery = 0.75f;
	[SerializeField] UnityEvent onPair;
	[SerializeField] UnityEvent onUnpair;
	
	TCPConnection currentlyPaired = null;
	string logs = "";
	bool sendStatus = false;

	void OnEnable() {
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable() {
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string message, string stackTrace, LogType type) {
		if(!enabled) return;

		if(logs.Length > 0) logs += "\n";
		switch(type) {
			case LogType.Error:
				logs += "[Error]\t";
				break;
			case LogType.Assert:
				logs += "[Assert]\t";
				break;
			case LogType.Warning:
				logs += "[Warning]\t";
				break;
			case LogType.Log:
				logs += "[Log]\t";
				break;
			case LogType.Exception:
				logs += "[Exception]\t";
				break;
			default:
				logs += "[???]\t";
				break;
		}
		logs += message;
	}

	public static VRAdapter Instance { get; private set; }

	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
		sendStatus = false;
	}


	public void OnNewConnection(TCPConnection connection) {
		SendPairConfirm();//give them our current pairing state
		if (currentlyPaired == null && TCPClient.Instance && connection.uniqueId == TCPClient.Instance.LockedId) {
			//we're locked to this one so let's immediately request connection
			SendAutopair(connection);
		}
		if(!sendStatus)
			StartCoroutine(sendStatusPeriodically());
	}

	public void OnConnectionEnd(TCPConnection connection) {
		if(connection == currentlyPaired) {
			connection.paired = false;
			currentlyPaired = null;
			onUnpair.Invoke();
			SendPairConfirm();//advertise as available to all guides
		}
		connection.active = false;
	}

	public void OnReceiveGuideLock(TCPConnection connection) {
		if(currentlyPaired != null && currentlyPaired == connection && TCPClient.Instance && TCPClient.Instance.LockedId != connection.uniqueId) {
			TCPClient.Instance.LockedId = connection.uniqueId;
			Haze.Logger.Log("Locking to " + connection);
			SendPairConfirm();
		} else {
			Haze.Logger.LogWarning("Cannot lock " + connection + ": currently paired with " + currentlyPaired);
		}
	}

	public void OnReceiveGuideUnlock(TCPConnection connection) {
		if(TCPClient.Instance) {
			if(TCPClient.Instance.LockedId != "free") {
				TCPClient.Instance.LockedId = "free";
				Haze.Logger.Log("Unlocking.");
				SendPairConfirm();
			}
		}
	}

	public void SendAutopair(TCPConnection connection) {
		if(currentlyPaired == null && TCPClient.Instance && connection.uniqueId == TCPClient.Instance.LockedId) {
			List<byte> data = new List<byte>();
			data.WriteString("autopair");
			Haze.Logger.Log("Sending autopair request");
			connection.Send(data);
		} else {
			Haze.Logger.LogWarning("Not allowed to send autopair to " + connection + "...");
		}
	}

	public void OnReceiveGuidePair(TCPConnection connection) {
		if(currentlyPaired == null && TCPClient.Instance && (TCPClient.Instance.LockedId == "free" || TCPClient.Instance.LockedId == connection.uniqueId)) {
			currentlyPaired = connection;
			connection.paired = true;
			Haze.Logger.Log("Paired to " + connection);
			onPair.Invoke();
			Autocalibration.Instance.OnPair();
			SendPairConfirm();
		} else {
			Haze.Logger.LogWarning("Cannot pair to " + connection);
		}
	}

	public void OnReceiveGuideUnpair(TCPConnection connection) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			currentlyPaired = null;
			connection.paired = false;
			Haze.Logger.Log("Unpaired.");
			onUnpair.Invoke();
			SendPairConfirm();
		}else if(currentlyPaired != null){
			Haze.Logger.LogWarning("Received Unpair message from a guide we're not paired to. Accepting request regardless.");
			OnReceiveGuideUnpair(currentlyPaired);//simulate receiving it from our paired guide instead
		} else {
			Haze.Logger.LogWarning("Cannot unpair.");
		}
	}

	public void SendPairConfirm() {
		List<byte> data = new List<byte>();
		data.WriteString("pair-confirm");
		data.WriteString(currentlyPaired != null ? currentlyPaired.uniqueId : "0");
		if (TCPClient.Instance)
			data.WriteString(TCPClient.Instance.LockedId);
		else
			data.WriteString("free");
		if (TCPClient.Instance)
			TCPClient.Instance.BroadcastToAllGuides(data);
		if (currentlyPaired != null) SendIsEmpty();
	}

	public void OnReceiveLogsQuery(TCPConnection connection) {
		if(connection == currentlyPaired) {
			SendLogs();
		}
	}

	public void SendLogs() {
		if(currentlyPaired != null) {
			List<byte> data = new List<byte>();
			data.WriteString("logs");
			data.WriteString(logs);
			logs = "";
			currentlyPaired.Send(data);
		}
	}

	public void OnReceiveHasVideo(TCPConnection connection, string videoName) {
		if(connection != null && connection == currentlyPaired) {
			if(VRVideoPlayer.IsVideoAvailable(videoName)) {
				SendHasVideoResponse(videoName);
			}
		}
	}

	public void SendHasVideoResponse(string videoName) {
		if(currentlyPaired != null) {
			List<byte> data = new List<byte>();
			data.WriteString("has-video-response");
			data.WriteString(videoName);
			currentlyPaired.Send(data);
		}
	}

	public void SendIsEmpty() {
		if(currentlyPaired != null) {
			List<byte> data = new List<byte>();
			data.WriteString("is-empty");
			currentlyPaired.Send(data);
		}
	}

	public async void OnReceiveLoadVideo(TCPConnection connection, string videoName, string mode) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.VideoLoadingResponse response = await VRVideoPlayer.Instance.LoadVideo(videoName, mode);
				SendLoadVideoResponse(response.ok, response.errorMessage);
			}
		}
	}

	public void SendLoadVideoResponse(bool ok, string errorMessage) {
		if(currentlyPaired != null) {
			List<byte> data = new List<byte>();
			data.WriteString("load-video-response");
			data.WriteBool(ok);
			data.WriteString(errorMessage);
			currentlyPaired.Send(data);
		}
	}

	public void OnReceivePlayVideo(TCPConnection connection, DateTime timestamp, float syncTime, Vector3 settings) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.Instance.PlayVideo(timestamp, syncTime, settings);
			}
		}
	}

	public void OnReceivePauseVideo(TCPConnection connection, DateTime unused, double videoTime, bool pause) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.Instance.PauseVideo(videoTime, pause);
			}
		}
	}

	public void OnReceiveStopVideo(TCPConnection connection) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.Instance.StopVideo();
			}
		}
	}

	public void SendSync(double videoTime) {
		if(currentlyPaired != null) {
			List<byte> data = new List<byte>();
			data.WriteString("sync");
			data.WriteTimestamp(DateTime.Now);
			data.WriteDouble(videoTime);
			currentlyPaired.Send(data);
		}
	}

	public void OnReceiveSync(TCPConnection connection, DateTime timestamp, double videoTime) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.Instance.Sync(timestamp, videoTime);
			}
		}
	}

	public void OnReceiveCalibrate(TCPConnection connection) {
		if(currentlyPaired != null && connection == currentlyPaired) {
			if(VRVideoPlayer.Instance)
				VRVideoPlayer.Instance.Recenter();
		}
	}

	public void OnReceiveStartChoice(TCPConnection connection, string question, string optionsDescriptions,  string optionsPositions) { //TODO must add positions
		if(currentlyPaired != null && connection == currentlyPaired) {
			Haze.Logger.Log("Display choices \'" + optionsDescriptions);
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.Instance.DisplayChoice(question, optionsDescriptions, optionsPositions);
			}
		}
	}

	public async void OnReceiveEditChoice(TCPConnection connection, string videoName, string description, string eulerAngles)
	{
		if (currentlyPaired != null && connection == currentlyPaired)
		{
			Haze.Logger.Log("Edit choice for video" + videoName + " ");
			//TODO load with mode (360 or not)
			VRVideoPlayer.Instance.OnEditOption(videoName,description, new Vector3());
			VRVideoPlayer.VideoLoadingResponse response = await VRVideoPlayer.Instance.LoadVideo(videoName, "360");
		}
	}

	public void OnReceiveSaveChoice(TCPConnection connection, string videoName, string description, string eulerAngles)
	{
		if (currentlyPaired != null && connection == currentlyPaired)
		{
			Haze.Logger.Log("Save choice for video" + videoName + " ");
			List<byte> data = new List<byte>();
			data.WriteString("choice-position");

			data.WriteString(Camera.main.gameObject.transform.eulerAngles.x + "," +
			                 Camera.main.gameObject.transform.eulerAngles.y + "," +
			                 Camera.main.gameObject.transform.eulerAngles.z);

			currentlyPaired.Send(data);
		}
	}
	
	public void SendSelectOption(byte option) { //TODO also remove the instantiated options, also when guide exits video
		if(currentlyPaired != null) {
			List<byte> data = new List<byte>();
			data.WriteString("select-option");
			data.WriteByte(option);
			currentlyPaired.Send(data);
		}
	}

	public void SendStatus(int battery, float fps, int temperature) {
		List<byte> data = new List<byte>();
		data.WriteString("status");
		data.WriteByte((byte)battery);
		data.WriteShort((short)(fps * 10));
		data.WriteByte((byte)temperature);
		if(TCPClient.Instance)
			TCPClient.Instance.BroadcastToAllGuides(data);
	}

	IEnumerator sendStatusPeriodically() {
		sendStatus = true;
		while(sendStatus && TCPClient.Instance && TCPClient.Instance.HasAtLeastOneActiveConnection && Status.Instance) {

			SendStatus(Status.Instance.Battery, Status.Instance.FPS, Status.Instance.Temperature);

			yield return new WaitForSeconds(sendStatusEvery);
		}
		sendStatus = false;
	}

	public void OnReceiveReorient(TCPConnection connection, Vector3 angles) {
		if(connection == currentlyPaired && connection != null) {
			if(VRVideoPlayer.Instance) {
				VRVideoPlayer.Instance.Reorient(angles);
			}
		}
	}

	public void OnReceiveAutocalibration(TCPConnection connection, byte command) {
		if(connection == currentlyPaired && connection != null) {
			if(Autocalibration.Instance)
				Autocalibration.Instance.OnReceiveAutocalibrate(command);
		}
	}

	public void SendAutocalibrationResult(byte command, float drift) {
		if(currentlyPaired != null && currentlyPaired.active) {
			List<byte> data = new List<byte>();
			data.WriteString("autocalibration-result");
			data.WriteByte(command);
			data.WriteFloat(drift);
			currentlyPaired.Send(data);
		}
	}

	public void ReceiveGotoTime(TCPConnection connection, double time) {
		if(currentlyPaired != null && currentlyPaired == connection) {
			if(VRVideoPlayer.Instance != null)
				VRVideoPlayer.Instance.Goto(time);
		}
	}

}
