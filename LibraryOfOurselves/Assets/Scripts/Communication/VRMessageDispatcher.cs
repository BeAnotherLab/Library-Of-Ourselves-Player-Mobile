using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class VRMessageDispatcher : MonoBehaviour
{ //Dispatches messages received by the VR headset

	[SerializeField] private bool ignoreIncorrectChannels = true;

	[SerializeField] private UnityEvent<TCPConnection> _guideLock; //VR
	[SerializeField] private UnityEvent<TCPConnection> _guideUnlock; //VR
	[SerializeField] private UnityEvent<TCPConnection> guidePair; //VR
	[SerializeField] private UnityEvent<TCPConnection> guideUnpair; //VR
	[SerializeField] private UnityEvent<TCPConnection> logsQuery; //VR
	[SerializeField] private UnityEvent<TCPConnection, string> hasVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string> loadVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, DateTime, float, Vector3> playVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, DateTime, double, bool> pauseVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection> stopVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection> calibrate; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string, string> startChoice; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string, string> editChoice; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string, string> saveChoice; //VR
	[SerializeField] private UnityEvent<TCPConnection, Vector3> reorient; //VR
	[SerializeField] private UnityEvent<TCPConnection, byte> autocalibration; //VR
	[SerializeField] private UnityEvent<TCPConnection, double> gotoTime; //VR

	public void OnMessageReception(TCPConnection connection, string channel, List<byte> data) 
	{
		if (channel == "guide-lock") _guideLock.Invoke(connection);
		else if (channel == "guide-unlock") _guideUnlock.Invoke(connection);
		else if (channel == "guide-pair") guidePair.Invoke(connection);
		else if (channel == "guide-unpair") guideUnpair.Invoke(connection);
		else if (channel == "logs-query") logsQuery.Invoke(connection);
		else if (channel == "has-video") hasVideo.Invoke(connection, data.ReadString());
		else if (channel == "load-video")
		{
			string videoName = data.ReadString();
			string mode = data.ReadString();
			loadVideo.Invoke(connection, videoName, mode);
		}
		else if (channel == "play-video")
		{
			DateTime stamp = data.ReadTimestamp();
			float syncTime = data.ReadFloat();
			Vector3 settings = data.ReadVector3();
			playVideo.Invoke(connection, stamp, syncTime, settings);	
		}
		else if (channel == "pause-video")
		{
			DateTime stamp = data.ReadTimestamp();
			double time = data.ReadDouble();
			bool pause = data.ReadBool();
			pauseVideo.Invoke(connection, stamp, time, pause);
		}
		else if (channel == "stop-video") stopVideo.Invoke(connection);
		else if (channel == "calibrate") calibrate.Invoke(connection);
		else if (channel == "start-choice") ParseChoiceMessage(connection, editChoice, data);
		else if (channel == "edit-choice") ParseChoiceMessage(connection, editChoice, data);
		else if (channel == "save-choice") ParseChoiceMessage(connection, saveChoice, data);
		else if (channel == "reorient") reorient.Invoke(connection, data.ReadVector3());
		else if (channel == "autocalibration") autocalibration.Invoke(connection, data.ReadByte());
		else if (channel == "goto") gotoTime.Invoke(connection, data.ReadDouble());
		else if (!ignoreIncorrectChannels) Haze.Logger.LogWarning("Received message on illegal channel: " + channel);
	}

	private void ParseChoiceMessage(TCPConnection connection, UnityEvent<TCPConnection, string, string, string> myEvent, List<byte> data)
	{
		string videoName = data.ReadString();
		string description = data.ReadString();
		string eulerAngles = data.ReadString();
	    myEvent.Invoke(connection, videoName, description, eulerAngles);
	}
	
}
