using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable] class GuideLockMessage : UnityEvent<TCPConnection> { }
[Serializable] class PairingMessage : UnityEvent<TCPConnection> { }
[Serializable] class PairConfirmationMessage : UnityEvent<TCPConnection, string> { }
[Serializable] class LogsMessage : UnityEvent<TCPConnection, string> { }
[Serializable] class VideoNameMessage : UnityEvent<TCPConnection, string> { }
[Serializable] class LoadVideoMessage : UnityEvent<TCPConnection, bool, string> { }
[Serializable] class VideoTimeStampMessage : UnityEvent<TCPConnection, DateTime> { }
[Serializable] class VideoTimeStampAndTimeMessage : UnityEvent<TCPConnection, DateTime, double> { }
[Serializable] class VideoPlaybackMessage : UnityEvent<TCPConnection> { }
[Serializable] class ChoiceStartMessage : UnityEvent<TCPConnection, string, string> { }
[Serializable] class ChoiceSelectMessage : UnityEvent<TCPConnection, byte> { }
[Serializable] class StatusMessage : UnityEvent<TCPConnection, byte, short, byte> { }

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
	[SerializeField] VideoNameMessage loadVideo;
	[SerializeField] LoadVideoMessage loadVideoResponse;
	[SerializeField] VideoTimeStampMessage playVideo;
	[SerializeField] VideoTimeStampAndTimeMessage pauseVideo;
	[SerializeField] VideoPlaybackMessage stopVideo;
	[SerializeField] VideoTimeStampAndTimeMessage sync;
	[SerializeField] VideoPlaybackMessage calibrate;
	[SerializeField] ChoiceStartMessage startChoice;
	[SerializeField] ChoiceSelectMessage selectOption;
	[SerializeField] StatusMessage status;

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
				string lockedId = data.ReadString();
				pairConfirm.Invoke(connection, lockedId);
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
					loadVideo.Invoke(connection, videoName);
				}
				break;
			case "load-video-response": {
					bool ok = data.ReadBool();
					string errorMessage = data.ReadString();
					loadVideoResponse.Invoke(connection, ok, errorMessage);
				}
				break;
			case "play-video": {
					DateTime stamp = data.ReadTimestamp();
					playVideo.Invoke(connection, stamp);
				}
				break;
			case "pause-video": {
					DateTime stamp = data.ReadTimestamp();
					double time = data.ReadDouble();
					pauseVideo.Invoke(connection, stamp, time);
				}
				break;
			case "stop-video":
				stopVideo.Invoke(connection);
				break;
			case "sync": {
					DateTime stamp = data.ReadTimestamp();
					double time = data.ReadDouble();
					sync.Invoke(connection, stamp, time);
				}
				break;
			case "calibrate":
				calibrate.Invoke(connection);
				break;
			case "start-choice": {
					string choice1 = data.ReadString();
					string choice2 = data.ReadString();
					startChoice.Invoke(connection, choice1, choice2);
				}
				break;
			case "select-option": {
					byte option = data.ReadByte();
					selectOption.Invoke(connection, option);
				}
				break;
			default:
				Debug.LogWarning("Received message on illegal channel: " + channel);
				break;
		}
	}

}
