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
	public IPEndPoint udpEndpoint = null;//if this is non-null, the connection has been established over udp rather than tcp
	public IPEndPoint sourceEndpoint = null;//the UDP endpoint through which the connection has been established.

	DateTime lastCommunication;

	List<List<byte>> udpPackets = new List<List<byte>>();

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

	public NetworkStream Stream { get { if(UDP) return null; return client.GetStream(); } }

	public bool UDP { get { return udpEndpoint != null; } }

	public TCPConnection() {
		lastCommunication = DateTime.Now;
	}

	public async Task Send(List<byte> bytes) {
		if(!active) {
			Haze.Logger.Log("Could not send bytes. Connection failed.");
			return;
		}
		try {
			if(UDP) {//Send over UDP
				await SendUDPPacket(bytes);
			} else {//Send over TCP
				if(bytes.Count > (int)short.MaxValue) {
					Haze.Logger.LogWarning("Bytes count is higher than max short value - sending first " + (short.MaxValue-2) + " bytes.");
					bytes.RemoveRange(short.MaxValue - 2, bytes.Count - short.MaxValue - 2);
				}
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
			}
		} catch(Exception e) {
			Haze.Logger.Log("Could not send bytes. Connection failed: " + e);
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
				if(!UDP) {
					//Receive through TCP
					byte[] lengthBuffer = new byte[2];
					await Stream.ReadAsync(lengthBuffer, 0, 2);
					byte len1 = lengthBuffer[0];
					byte len2 = lengthBuffer[1];
					short length = ToShort(len1, len2);
					byte[] buffer = new byte[length];
					await Stream.ReadAsync(buffer, 0, length);
					lastCommunication = DateTime.Now;
					return new List<byte>(buffer);
				} else {
					//Return anything we've received through ReceiveUDPPacket
					while(udpPackets.Count <= 0) {
						await Task.Delay(30);//Wait for a packet to reach
					}
					List<byte> nextPacket = udpPackets[0];
					udpPackets.RemoveAt(0);
					lastCommunication = DateTime.Now;
					return nextPacket;
				}
			}
		}catch(Exception e) {
			Haze.Logger.Log("Could not receive bytes. Connection failed.");
		}
		//if we get here, it means we've not returned yet...
		active = false;//this will notify client or host to disconnect from this connection
		List<byte> fakeData = new List<byte>();
		fakeData.WriteString("disconnection");
		return fakeData;
	}

	//Called upon receiving a UDP packet addressed to this TCPConnection
	public void ReceiveUDPPacket(List<byte> data) {
		udpPackets.Add(data);
	}

	async Task SendUDPPacket(List<byte> data) {
		UDPListener listener = UDPListener.Instance;
		UDPBroadcaster broadcaster = UDPBroadcaster.Instance;
		if(listener != null) {
			await listener.SendUDPMessage(udpEndpoint, data.ToArray());
		} else if(broadcaster != null) {
			await broadcaster.SendUDPMessage(udpEndpoint, data.ToArray());
		} else {
			throw new Exception("No available UDP channel to send the message through!");
		}
	}

	public override string ToString() {
		return "[" + deviceType + ": " + xrDeviceModel + "-" + uniqueId + "]";
	}

	~TCPConnection() {
		Haze.Logger.Log("Removed connection " + this + " (" + sourceEndpoint + ")");
		if(UDPListener.Instance)
			UDPListener.Instance.RemoveEncounteredIP(sourceEndpoint);

		if(GuideAdapter.Instance) {
			GuideAdapter.Instance.onConnectionEnd.Invoke(this);
		}
	}
}
