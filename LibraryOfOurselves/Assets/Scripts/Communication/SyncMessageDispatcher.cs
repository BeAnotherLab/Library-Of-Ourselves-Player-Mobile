using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class SyncMessageDispatcher : MonoBehaviour
{ //TODO separate Guide/VR messages or at least make regions for each
	[SerializeField] private UnityEvent<TCPConnection, DateTime, double> sync; //Guide and VR
	
	public void OnMessageReception(TCPConnection connection, string channel, List<byte> data) 
	{
		if (channel == "sync")
		{			
			DateTime stamp = data.ReadTimestamp();
			double time = data.ReadDouble();
			sync.Invoke(connection, stamp, time);
		}
	}

}
