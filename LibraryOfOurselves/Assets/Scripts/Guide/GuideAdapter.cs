
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAdapter : MonoBehaviour{

	public void OnNewConnection(TCPConnection connection) {
		Debug.Log(connection + " has been added as an option");
	}

	public void OnConnectionEnd(TCPConnection connection) {
		connection.active = false;
		connection.paired = false;
		Debug.Log(connection + " is no longer available");
	}

	public async void SendGuideLock(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-lock");
		await connection.Send(data);
	}

	public async void SendGuideUnlock(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-unlock");
		await connection.Send(data);
	}

	public void OnReceiveAutopair(TCPConnection connection) {
		Debug.Log(connection.ToString() + " requests autopair.");
		SendGuidePair(connection);
	}

	public async void SendGuidePair(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-pair");
		await connection.Send(data);
	}

	public async void SendGuideUnpair(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("guide-unpair");
		await connection.Send(data);
	}

	public void OnReceivePairConfirm(TCPConnection connection, string uniqueId) {
		bool paired = uniqueId != "0";//string "0" means the device is unpaired now
		if(uniqueId == SystemInfo.deviceUniqueIdentifier) {
			Debug.Log("Paired to " + connection);
			connection.paired = paired;
		} else {
			//might want to take note that the device is available/unavailable now (depending on value of paired)
		}
	}

	public async void SendLogsQuery(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("logs-query");
		await connection.Send(data);
	}

	public void OnReceiveLogs(TCPConnection connection, string log) {
		Debug.Log("Received logs from "+connection+":\n" + log);
	}

	public void SendHasVideo(string videoName) {
		List<byte> data = new List<byte>();
		data.WriteString("has-video");
		data.WriteString(videoName);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void OnReceiveHasVideoResponse(TCPConnection connection, string videoName) {
		Debug.Log(connection + " has video " + videoName);
	}

	public void OnReceiveIsEmpty(TCPConnection connection) {
		Debug.Log(connection + " is empty");
	}

	public void SendLoadVideo(string videoName) {
		List<byte> data = new List<byte>();
		data.WriteString("load-video");
		data.WriteString(videoName);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void OnReceiveLoadVideoResponse(TCPConnection connection, bool ok, string errorMessage) {
		if(ok) {
			Debug.Log(connection + " has successfully loaded video.");
		} else {
			Debug.Log("Error: " + connection + " could not load video: " + errorMessage);
		}
	}

	public void SendPlayVideo() {
		List<byte> data = new List<byte>();
		data.WriteString("play-video");
		data.WriteTimestamp(DateTime.Now);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void SendPauseVideo(double videoTime) {
		List<byte> data = new List<byte>();
		data.WriteString("pause-video");
		data.WriteTimestamp(DateTime.Now);
		data.WriteDouble(videoTime);
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

	public async void SendCalibrate(TCPConnection connection) {
		List<byte> data = new List<byte>();
		data.WriteString("calibrate");
		await connection.Send(data);
	}

	public void SendStartChoice(string choice1, string choice2) {
		List<byte> data = new List<byte>();
		data.WriteString("start-choice");
		data.WriteString(choice1);
		data.WriteString(choice2);
		if(TCPHost.Instance)
			TCPHost.Instance.BroadcastToPairedDevices(data);
	}

	public void OnReceiveSelectOption(TCPConnection connection, byte option) {
		Debug.Log(connection + " has selected option " + option);
	}

	public void OnReceiveStatus(TCPConnection connection, byte b_battery, short s_fps, byte b_temp) {
		int battery = (int)b_battery;
		float fps = ((float)s_fps) * 0.1f;
		int temperature = (int)b_temp;
		Debug.Log(connection + " status; battery " + battery + " / fps " + fps + " / temperature " + temperature);
	}

}
