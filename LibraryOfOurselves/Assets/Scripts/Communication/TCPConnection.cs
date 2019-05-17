using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TCPConnection {
	public string uniqueId;
	public TcpClient client;
	public NetworkStream stream;

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
		await stream.WriteAsync(data, 0, data.Length);
		Debug.Log("Sent " + length + " bytes.");
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
		await stream.ReadAsync(lengthBuffer, 0, 2);
		byte len1 = lengthBuffer[0];
		byte len2 = lengthBuffer[1];
		short length = ToShort(len1, len2);
		byte[] buffer = new byte[length];
		await stream.ReadAsync(buffer, 0, length);
		Debug.Log("Received " + length + " bytes");
		return new List<byte>(buffer);
	}
}
