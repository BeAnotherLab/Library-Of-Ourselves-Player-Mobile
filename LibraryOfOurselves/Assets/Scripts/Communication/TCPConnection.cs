using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine.Events;

[Serializable] public class NewConnectionEvent : UnityEvent<TCPConnection> { }
[Serializable] public class MessageReceivedEvent : UnityEvent<TCPConnection, string, List<byte>> { }
[Serializable] public class ConnectionEndEvent : UnityEvent<TCPConnection> { }
[Serializable] public class ResponsivenessEvent : UnityEvent<TCPConnection, bool> { }

public class TCPConnection {

	public string uniqueId;//the unique id of the device on the other end
	public string xrDeviceModel = "unknown";//the XR device model name
	public TcpClient client;//the TCP client used to communicate with the other end
	public DeviceType deviceType;//whichever device type it is on the other end
	public bool active = true;//set to false once connection is closed and shouldn't be used anymore
	public bool paired = false;//set to true for devices paired to this one
	public string lockedId = "free";//for VR and AUDIO devices, either "free" or a guide's uniqueId to allow pairing with.
	public bool responsive = true;//after not receiving anything for a while, will go to "unresponsive"

	DateTime lastCommunication;

	public float TimeSinceLastConnection {
		get {
			return (float)(DateTime.Now - lastCommunication).TotalSeconds;
		}
	}

	public enum DeviceType {
		GUIDE,
		VR,
		AUDIO
	}

	public NetworkStream Stream { get { return client.GetStream(); } }

	public TCPConnection() {
		lastCommunication = DateTime.Now;
	}

	public async Task Send(List<byte> bytes) {
		if(!active) {
			Debug.Log("Could not send bytes. Connection failed.");
			return;
		}
		try {
			short length = (short)bytes.Count;
			byte len1, len2;
			FromShort(length, out len1, out len2);
			byte[] data = new byte[length + 2];
			data[0] = len1;
			data[1] = len2;
			for(int i = 0; i < length; ++i) {
				data[i + 2] = bytes[i];
			}
			await Stream.WriteAsync(data, 0, data.Length);
		} catch(Exception e) {
			Debug.Log("Could not send bytes. Connection failed.");
			active = false;//this will notify client or host to disconnect from this connection.
		}
	}

	static short ToShort(short byte1, short byte2) {
		return (short)((byte2 << 8) + byte1);
	}

	static void FromShort(short number, out byte byte1, out byte byte2) {
		byte2 = (byte)(number >> 8);
		byte1 = (byte)(number & 255);
	}

	public async Task<List<byte>> Receive() {
		try {
			if(active) {
				byte[] lengthBuffer = new byte[2];
				await Stream.ReadAsync(lengthBuffer, 0, 2);
				byte len1 = lengthBuffer[0];
				byte len2 = lengthBuffer[1];
				short length = ToShort(len1, len2);
				byte[] buffer = new byte[length];
				await Stream.ReadAsync(buffer, 0, length);
				lastCommunication = DateTime.Now;
				return new List<byte>(buffer);
			}
		}catch(Exception e) {
			Debug.Log("Could not receive bytes. Connection failed.");
		}
		//if we get here, it means we've not returned yet...
		active = false;//this will notify client or host to disconnect from this connection
		List<byte> fakeData = new List<byte>();
		fakeData.WriteString("disconnection");
		return fakeData;
	}

	public override string ToString() {
		return "[" + deviceType + ": " + uniqueId + "]";
	}
}
