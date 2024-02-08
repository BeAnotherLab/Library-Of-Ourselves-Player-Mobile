using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class GuideMessageDispatcher : MonoBehaviour
{ //dispatches messages received by the guide tablet 

	[SerializeField] private bool ignoreIncorrectChannels = true;

	[SerializeField] private UnityEvent<TCPConnection> autopair; //guide
	[SerializeField] private UnityEvent<TCPConnection, string, string> pairConfirm; //guide 
	[SerializeField] private UnityEvent<TCPConnection, string>  logs; //Guide
	[SerializeField] private UnityEvent<TCPConnection, string> hasVideoResponse; //Guide
	[SerializeField] private UnityEvent<TCPConnection> isEmpty; //Guide
	[SerializeField] private UnityEvent<TCPConnection, bool, string> loadVideoResponse; //Guide
	[SerializeField] private UnityEvent<TCPConnection, byte> selectOption; //Guide 
	[SerializeField] private UnityEvent<TCPConnection, string> choicePositionMessage; //Guide
	[SerializeField] private UnityEvent<TCPConnection, byte, short, byte> status; //Guide

	public void OnMessageReception(TCPConnection connection, string channel, List<byte> data) 
	{
		if (channel == "status") {
			byte battery = data.ReadByte();
			short fps = data.ReadShort();
			byte temp = data.ReadByte();
			status.Invoke(connection, battery, fps, temp);
		}
		else if (channel == "autopair" ) autopair.Invoke(connection);
		else if (channel == "pair-confirm") //can either confirm pairing on unpairing
		{
			string pairedId = data.ReadString();
			string lockedId = data.ReadString();
			pairConfirm.Invoke(connection, pairedId, lockedId);	
		}
		else if (channel == "logs") logs.Invoke(connection, data.ReadString());
		else if (channel == "has-video-response")
		{
			string videoName = data.ReadString();
			hasVideoResponse.Invoke(connection, videoName);
		}
		else if (channel == "is-empty") isEmpty.Invoke(connection);
		else if (channel == "load-video-response")
		{
			bool ok = data.ReadBool();
			string errorMessage = data.ReadString();
			loadVideoResponse.Invoke(connection, ok, errorMessage);
		}
		else if (channel == "select-option")
		{
			byte option = data.ReadByte();
			selectOption.Invoke(connection, option);
		}
		else if (channel == "choice-position") choicePositionMessage.Invoke(connection, data.ReadString());    
	}

}
