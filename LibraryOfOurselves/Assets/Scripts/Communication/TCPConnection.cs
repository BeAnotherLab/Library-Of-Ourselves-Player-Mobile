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

public class TCPConnection {

	public string uniqueId;//the unique id of the device on the other end
	public TcpClient client;//the TCP client used to communicate with the other end
	public DeviceType deviceType;//whichever device type it is on the other end
	public bool active = true;//set to false once connection is closed and shouldn't be used anymore
	public bool paired = false;//set to true for devices paired to this one
	public string lockedId = "free";//for VR and AUDIO devices, either "free" or a guide's uniqueId to allow pairing with.

	public enum DeviceType {
		GUIDE,
		VR,
		AUDIO
	}

	public NetworkStream Stream { get { return client.GetStream(); } }

	public async Task Send(List<byte> bytes) {
		short length = (short)bytes.Count;
		byte len1, len2;
		FromShort(length, out len1, out len2);
		byte[] data = new byte[length + 2];
		data[0] = len1;
		data[1] = len2;
		for(int i = 0; i<length; ++i) {
			data[i + 2] = bytes[i];
		}
		await Stream.WriteAsync(data, 0, data.Length);
	}

	static short ToShort(short byte1, short byte2) {
		return (short)((byte2 << 8) + byte1);
	}

	static void FromShort(short number, out byte byte1, out byte byte2) {
		byte2 = (byte)(number >> 8);
		byte1 = (byte)(number & 255);
	}

	public async Task<List<byte>> Receive() {
		byte[] lengthBuffer = new byte[2];
		await Stream.ReadAsync(lengthBuffer, 0, 2);
		byte len1 = lengthBuffer[0];
		byte len2 = lengthBuffer[1];
		short length = ToShort(len1, len2);
		byte[] buffer = new byte[length];
		await Stream.ReadAsync(buffer, 0, length);
		return new List<byte>(buffer);
	}

	public override string ToString() {
		return "[" + deviceType + ": " + uniqueId + "]";
	}
}
