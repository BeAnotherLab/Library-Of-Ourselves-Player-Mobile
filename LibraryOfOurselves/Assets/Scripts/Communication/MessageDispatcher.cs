﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable] class GuideLockMessage : UnityEvent<TCPConnection> { }
[Serializable] class PairingMessage : UnityEvent<TCPConnection> { }
[Serializable] class PairConfirmationMessage : UnityEvent<TCPConnection, string, string> { }
[Serializable] class LogsMessage : UnityEvent<TCPConnection, string> { }
[Serializable] class VideoNameMessage : UnityEvent<TCPConnection, string> { }
[Serializable] class LoadVideoMessage : UnityEvent<TCPConnection, string, string> { }
[Serializable] class LoadVideoResponseMessage : UnityEvent<TCPConnection, bool, string> { }
[Serializable] class VideoTimeStampMessage : UnityEvent<TCPConnection, DateTime, float, Vector3> { }
[Serializable] class VideoTimeStampTimeBoolMessage : UnityEvent<TCPConnection, DateTime, double, bool> { }
[Serializable] class VideoTimeStampAndTimeMessage : UnityEvent<TCPConnection, DateTime, double> { }
[Serializable] class VideoTimeMessage : UnityEvent<TCPConnection, double> { }
[Serializable] class VideoPlaybackMessage : UnityEvent<TCPConnection> { }
[Serializable] class ChoiceStartMessage : UnityEvent<TCPConnection, string, string, string> { }
[Serializable] class ChoiceSelectMessage : UnityEvent<TCPConnection, byte> { }
[Serializable] class ReorientMessage : UnityEvent<TCPConnection, Vector3> { }
[Serializable] class StatusMessage : UnityEvent<TCPConnection, byte, short, byte> { }
[Serializable] class AutocalibrationMessage : UnityEvent<TCPConnection, byte> { }
[Serializable] class AutocalibrationResultMessage : UnityEvent<TCPConnection, byte, float> { }

public class MessageDispatcher : MonoBehaviour{

	[SerializeField] GuideLockMessage guideLock;
	[SerializeField] GuideLockMessage guideUnlock;
	[SerializeField] PairingMessage autopair;
	[SerializeField] PairingMessage guidePair;
	[SerializeField] PairingMessage guideUnpair;
	[SerializeField] PairConfirmationMessage pairConfirm;
	[SerializeField] PairingMessage logsQuery;
	[SerializeField] LogsMessage logs;
	[SerializeField] VideoNameMessage hasVideo;
	[SerializeField] VideoNameMessage hasVideoResponse;
	[SerializeField] PairingMessage isEmpty;
	[SerializeField] LoadVideoMessage loadVideo;
	[SerializeField] LoadVideoResponseMessage loadVideoResponse;
	[SerializeField] VideoTimeStampMessage playVideo;
	[SerializeField] VideoTimeStampTimeBoolMessage pauseVideo;
	[SerializeField] VideoPlaybackMessage stopVideo;
	[SerializeField] VideoTimeStampAndTimeMessage sync;
	[SerializeField] VideoPlaybackMessage calibrate;
	[SerializeField] ChoiceStartMessage startChoice;
	[SerializeField] ChoiceSelectMessage selectOption;
	[SerializeField] ReorientMessage reorient;
	[SerializeField] StatusMessage status;
	[SerializeField] AutocalibrationMessage autocalibration;
	[SerializeField] AutocalibrationResultMessage autocalibrationResult;
	[SerializeField] VideoTimeMessage gotoTime;
	[SerializeField] bool ignoreIncorrectChannels = true;

	private string dataToReadableFormat(List<byte> data) {
		string result = "{";
		for(int i = 0; i<data.Count; ++i) {
			result += data[i];
			if(i < data.Count - 1)
				result += ", ";
		}
		return result + "}";
	}

	public void OnMessageReception(TCPConnection connection, string channel, List<byte> data) {
		switch(channel) {
			case "status": {
					byte battery = data.ReadByte();
					short fps = data.ReadShort();
					byte temp = data.ReadByte();
					status.Invoke(connection, battery, fps, temp);
				}
				break;
			case "guide-lock":
				guideLock.Invoke(connection);
				break;
			case "guide-unlock":
				guideUnlock.Invoke(connection);
				break;
			case "autopair":
				autopair.Invoke(connection);
				break;
			case "guide-pair":
				guidePair.Invoke(connection);
				break;
			case "guide-unpair":
				guideUnpair.Invoke(connection);
				break;
			case "pair-confirm":
				string pairedId = data.ReadString();
				string lockedId = data.ReadString();
				pairConfirm.Invoke(connection, pairedId, lockedId);
				break;
			case "logs-query":
				logsQuery.Invoke(connection);
				break;
			case "logs": {
					string log = data.ReadString();
					logs.Invoke(connection, log);
				}
				break;
			case "has-video": {
					string videoName = data.ReadString();
					hasVideo.Invoke(connection, videoName);
				}
				break;
			case "has-video-response": {
					string videoName = data.ReadString();
					hasVideoResponse.Invoke(connection, videoName);
				}
				break;
			case "is-empty":
				isEmpty.Invoke(connection);
				break;
			case "load-video": {
					string videoName = data.ReadString();
					string mode = data.ReadString();
					loadVideo.Invoke(connection, videoName, mode);
				}
				break;
			case "load-video-response": {
					bool ok = data.ReadBool();
					string errorMessage = data.ReadString();
					loadVideoResponse.Invoke(connection, ok, errorMessage);
				}
				break;
			case "play-video": {
					Haze.Logger.Log("Received play-video. Deciphering time stamp. Data: " + dataToReadableFormat(data));
					DateTime stamp = data.ReadTimestamp();
					float syncTime = data.ReadFloat();
					Vector3 settings = data.ReadVector3();
					playVideo.Invoke(connection, stamp, syncTime, settings);
				}
				break;
			case "pause-video": {
					Haze.Logger.Log("Received pause-video. Deciphering time stamp. Data: " + dataToReadableFormat(data));
					DateTime stamp = data.ReadTimestamp();
					double time = data.ReadDouble();
					bool pause = data.ReadBool();
					pauseVideo.Invoke(connection, stamp, time, pause);
				}
				break;
			case "stop-video":
				stopVideo.Invoke(connection);
				break;
			case "sync": {
					Haze.Logger.Log("Received sync. Deciphering time stamp. Data: " + dataToReadableFormat(data));
					DateTime stamp = data.ReadTimestamp();
					double time = data.ReadDouble();
					sync.Invoke(connection, stamp, time);
				}
				break;
			case "calibrate":
				calibrate.Invoke(connection);
				break;
			case "start-choice": {
					string question = data.ReadString();
					string choice1 = data.ReadString();
					string choice2 = data.ReadString();
					startChoice.Invoke(connection, question, choice1, choice2);
				}
				break;
			case "select-option": {
					byte option = data.ReadByte();
					selectOption.Invoke(connection, option);
				}
				break;
			case "reorient": {
					Vector3 angles = data.ReadVector3();
					reorient.Invoke(connection, angles);
				}
				break;
			case "autocalibration": {
					byte command = data.ReadByte();
					autocalibration.Invoke(connection, command);
				}
				break;
			case "autocalibration-result": {
					byte command = data.ReadByte();
					float drift = data.ReadFloat();
					autocalibrationResult.Invoke(connection, command, drift);
				}
				break;
			case "goto": {
					double time = data.ReadDouble();
					gotoTime.Invoke(connection, time);
				}
				break;
			default:
				if(!ignoreIncorrectChannels)
					Haze.Logger.LogWarning("Received message on illegal channel: " + channel);
				break;
		}
	}

}
