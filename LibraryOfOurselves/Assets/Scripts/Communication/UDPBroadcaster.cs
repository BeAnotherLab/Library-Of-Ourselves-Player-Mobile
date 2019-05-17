using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//Sends UDP messages to the whole local network periodically

public class UDPBroadcaster : MonoBehaviour{

	[SerializeField] int broadcastPort = 9725;

	UdpClient server;
	byte[] requestData;
	bool stop = false;

	public async void StartBroadcasting(string ip, int port) {
		stop = false;
		server = new UdpClient(broadcastPort);
		requestData = Encoding.ASCII.GetBytes("guide-broadcast>"+ip+">"+port+">"+SystemInfo.deviceUniqueIdentifier);
		IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
		server.EnableBroadcast = true;
		while(!stop) {
			try {
				await server.SendAsync(requestData, requestData.Length, broadcastEndpoint);
				await Task.Delay(1000);
			}catch(SocketException se) {
				Debug.LogWarning("Socket error (" + se.ErrorCode + "), could not broadcast: " + se.ToString());
			}catch(Exception e) {
				Debug.LogWarning("Error, could not broadcast: " + e.ToString());
			}
		}
		server.Close();
	}

	public void StopBroadcasting() {
		stop = true;
	}

	private void OnDestroy() {
		StopBroadcasting();
	}

}
