using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class MessageDispatcher : MonoBehaviour
{ //TODO separate Guide/VR messages or at least make regions for each

	[SerializeField] private bool ignoreIncorrectChannels = true;

	[SerializeField] private UnityEvent<TCPConnection> _guideLock; //VR
	[SerializeField] private UnityEvent<TCPConnection> _guideUnlock; //VR
	[SerializeField] private UnityEvent<TCPConnection> autopair; //guide
	[SerializeField] private UnityEvent<TCPConnection> guidePair; //VR
	[SerializeField] private UnityEvent<TCPConnection> guideUnpair; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string> pairConfirm; //guide 
	[SerializeField] private UnityEvent<TCPConnection> logsQuery; //VR
	[SerializeField] private UnityEvent<TCPConnection, string>  logs; //Guide
	[SerializeField] private UnityEvent<TCPConnection, string> hasVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, string> hasVideoResponse; //Guide
	[SerializeField] private UnityEvent<TCPConnection> isEmpty; //Guide
	[SerializeField] private UnityEvent<TCPConnection, string, string> loadVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, bool, string> loadVideoResponse; //Guide
	[SerializeField] private UnityEvent<TCPConnection, DateTime, float, Vector3> playVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, DateTime, double, bool> pauseVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection> stopVideo; //VR
	[SerializeField] private UnityEvent<TCPConnection, DateTime, double> sync; //Guide and VR
	[SerializeField] private UnityEvent<TCPConnection> calibrate; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string, string> startChoice; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string, string> editChoice; //VR
	[SerializeField] private UnityEvent<TCPConnection, string, string, string> saveChoice; //VR
	[SerializeField] private UnityEvent<TCPConnection, byte> selectOption; //Guide 
	[SerializeField] private UnityEvent<TCPConnection, string> choicePositionMessage; //Guide
	[SerializeField] private UnityEvent<TCPConnection, Vector3> reorient; //VR
	[SerializeField] private UnityEvent<TCPConnection, byte, short, byte> status; //Guide
	[SerializeField] private UnityEvent<TCPConnection, byte> autocalibration; //VR
	[SerializeField] private UnityEvent<TCPConnection, byte, float> autocalibrationResult; //Guide
	[SerializeField] private UnityEvent<TCPConnection, double> gotoTime; //VR

	public void OnMessageReception(TCPConnection connection, string channel, List<byte> data) 
	{
		if (channel == "status") {
					byte battery = data.ReadByte();
					short fps = data.ReadShort();
					byte temp = data.ReadByte();
					status.Invoke(connection, battery, fps, temp);
		}
		else if (channel == "guide-lock") _guideLock.Invoke(connection);
		else if (channel == "guide-unlock") _guideUnlock.Invoke(connection);
		else if (channel == "autopair" ) autopair.Invoke(connection);
		else if (channel == "guide-pair") guidePair.Invoke(connection);
		else if (channel == "guide-unpair") guideUnpair.Invoke(connection);
		else if (channel == "pair-confirm")
		{
			string pairedId = data.ReadString();
			string lockedId = data.ReadString();
			pairConfirm.Invoke(connection, pairedId, lockedId);	
		}
		else if (channel == "logs-query") logsQuery.Invoke(connection);
		else if (channel == "logs") logs.Invoke(connection, data.ReadString());
		else if (channel == "has-video") hasVideo.Invoke(connection, data.ReadString());
		else if (channel == "has-video-response")
		{
			string videoName = data.ReadString();
			hasVideoResponse.Invoke(connection, videoName);
		}
		else if (channel == "is-empty") isEmpty.Invoke(connection);
		else if (channel == "load-video")
		{
			string videoName = data.ReadString();
			string mode = data.ReadString();
			loadVideo.Invoke(connection, videoName, mode);
		}
		else if (channel == "load-video-response")
		{
			bool ok = data.ReadBool();
			string errorMessage = data.ReadString();
			loadVideoResponse.Invoke(connection, ok, errorMessage);
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
		else if (channel == "sync")
		{			
			DateTime stamp = data.ReadTimestamp();
			double time = data.ReadDouble();
			sync.Invoke(connection, stamp, time);
		}
		else if (channel == "calibrate") calibrate.Invoke(connection);
		else if (channel == "start-choice")
		{
			string question = data.ReadString();
			string optionsDescriptions = data.ReadString();
			string optionsPositions = data.ReadString();
			startChoice.Invoke(connection, question, optionsDescriptions, optionsPositions);
		}
		else if (channel == "edit-choice")
		{
			string videoName = data.ReadString();
			string description = data.ReadString();
			string eulerAngles = data.ReadString();
			editChoice.Invoke(connection, videoName, description, eulerAngles);
		}
		else if (channel == "save-choice")
		{
			string videoName = data.ReadString();
			string description = data.ReadString();
			string eulerAngles = data.ReadString();
			saveChoice.Invoke(connection, videoName, description,eulerAngles);
		}
		else if (channel == "select-option")
		{
			byte option = data.ReadByte();
			selectOption.Invoke(connection, option);
		}
		else if (channel == "choice-position") choicePositionMessage.Invoke(connection, data.ReadString());    
		else if (channel == "reorient") reorient.Invoke(connection, data.ReadVector3());
		else if (channel == "autocalibration") autocalibration.Invoke(connection, data.ReadByte());
		else if (channel == "autocalibration-result")
		{
			byte command = data.ReadByte();
			float drift = data.ReadFloat();
			autocalibrationResult.Invoke(connection, command, drift);
		}
		else if (channel == "goto") gotoTime.Invoke(connection, data.ReadDouble());
		else if (!ignoreIncorrectChannels) Haze.Logger.LogWarning("Received message on illegal channel: " + channel);
	}

}
