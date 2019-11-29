using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

//Sends UDP messages to the whole local network periodically

public class UDPBroadcaster : MonoBehaviour{

	[SerializeField] int broadcastPort = 9725;

	UdpClient server;
	string localIP;
	int localPort;
	byte[] requestData;
	bool stop = false;

	bool __criticalError = false;
	public bool CriticalError {
		get { return __criticalError; }
		private set { __criticalError = value; }
	}

	public static UDPBroadcaster Instance { get; private set; }

	private void Start() {
		Instance = this;
	}

	public void StartBroadcasting(string ip, int port) {
		StartCoroutine(broadcast(ip, port));
	}

	IEnumerator broadcast(string ip, int port) {

		CriticalError = false;

		localIP = ip;
		localPort = port;

		stop = false;
		server = new UdpClient(broadcastPort);
		requestData = Encoding.ASCII.GetBytes("guide-broadcast>" + ip + ">" + port + ">" + SystemInfo.deviceUniqueIdentifier);
		IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
		server.EnableBroadcast = true;
		server.DontFragment = true;
		Listen();
		int exceptions = 0;
		while(!stop) {
			try {
				server.Send(requestData, requestData.Length, broadcastEndpoint);
				exceptions = 0;
			} catch(SocketException se) {
				Haze.Logger.LogWarning("Socket error (" + se.ErrorCode + "), could not broadcast: " + se.ToString());
				++exceptions;
			} catch(Exception e) {
				Haze.Logger.LogWarning("Error, could not broadcast: " + e.ToString());
				++exceptions;
			}

			/// Too many exceptions in a row
			if(exceptions > 7) {
				Haze.Logger.LogError("Critical network error: Broadcasting has failed 7 times in a row.");
				CriticalError = true;//this will flag the TCPHost to restart entirely
			}

			yield return new WaitForSeconds(1);
		}
		server.Close();
	}

	public async Task SendUDPMessage(IPEndPoint remote, byte[] data) {
		for(int i = 0; i < 2; ++i) {//send the message twice to be certain it reaches.
			try {
				await server.SendAsync(data, data.Length, remote);
			} catch(SocketException se) {
				Haze.Logger.LogWarning("[UDPBrodcaster] Socket error " + se.ErrorCode + ", cannot send UDP packet: " + se.ToString());
			} catch(Exception e) {
				Haze.Logger.LogWarning("[UDPBroadcaster] Error, cannot send UDP packet: " + e.ToString());
			}
		}
	}

	public void StopBroadcasting() {
		stop = true;
	}

	private void OnDestroy() {
		Instance = null;
		StopBroadcasting();
	}

	async void Listen() {
		while(!stop) {
			try {
				UdpReceiveResult result = await server.ReceiveAsync();
				List<byte> data = result.Buffer.ToList();
				TCPHost host = TCPHost.Instance;
				if(host != null) {
					host.ReceiveUDPPacket(result.RemoteEndPoint, data);
				}
			}catch(SocketException se) {
				Haze.Logger.LogWarning("[UDPBroadcaster] Socket error " + se.ErrorCode + ", cannot receive UDP packet: " + se.ToString());
			}catch(Exception e) {
				Haze.Logger.LogWarning("[UDPBroadcaster] Error, cannot receive UDP packet: " + e.ToString());
			}
		}
	}

	string byteArrayToString(byte[] bytes) {
		string ret = "[";
		for(int i = 0; i<bytes.Length; ++i) {
			ret += bytes[i];
			if(i < bytes.Length - 1)
				ret += ", ";
		}
		ret += "]";
		return ret;
	}

}
