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

	public void StartBroadcasting(string ip, int port) {
		StartCoroutine(broadcast(ip, port));
	}

	IEnumerator broadcast(string ip, int port) {

		localIP = ip;
		localPort = port;

		stop = false;
		server = new UdpClient(broadcastPort);
		requestData = Encoding.ASCII.GetBytes("guide-broadcast>" + ip + ">" + port + ">" + SystemInfo.deviceUniqueIdentifier);
		IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
		server.EnableBroadcast = true;
		server.DontFragment = true;
		Listen();
		while(!stop) {
			try {
				server.Send(requestData, requestData.Length, broadcastEndpoint);
			} catch(SocketException se) {
				Debug.LogWarning("Socket error (" + se.ErrorCode + "), could not broadcast: " + se.ToString());
			} catch(Exception e) {
				Debug.LogWarning("Error, could not broadcast: " + e.ToString());
			}

			yield return new WaitForSeconds(1);
		}
		server.Close();
	}

	public void StopBroadcasting() {
		stop = true;
	}

	private void OnDestroy() {
		StopBroadcasting();
	}

	async void Listen() {
		while(!stop) {
			try {
				UdpReceiveResult result = await server.ReceiveAsync();
				if(server.Client.LocalEndPoint != result.RemoteEndPoint) {
					List<byte> data = result.Buffer.ToList();
					string message = data.ReadString();
					Debug.Log("Received from " + result.RemoteEndPoint + ": " + message);
				}
			}catch(SocketException se) {
				Debug.LogWarning("[UDPBroadcaster] Socket error " + se.ErrorCode + ", cannot receive UDP packet: " + se.ToString());
			}catch(Exception e) {
				Debug.LogWarning("[UDPBroadcaster] Error, cannot receive UDP packet: " + e.ToString());
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
