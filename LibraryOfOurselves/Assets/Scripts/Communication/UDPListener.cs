using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class UDPListener : MonoBehaviour{

	[SerializeField] int listeningPort = 9725;
	[SerializeField] TCPClient tcpClient;

	UdpClient client = null;
	List<IPEndPoint> encounteredIPs = new List<IPEndPoint>();

	public static UDPListener Instance { get; private set; }

	async void Start() {
		Instance = this;
		client = new UdpClient(listeningPort);
		client.DontFragment = true;
		while (client != null) {
			try {
				UdpReceiveResult serverData = await client.ReceiveAsync();
				if (!tcpClient.ReceiveFakeTCPMessage(serverData.RemoteEndPoint, serverData.Buffer)) {//might want to use this as part of a pre-established UDP connection
					string message = Encoding.ASCII.GetString(serverData.Buffer);
					//Extract IP and port from message
					string[] splitMessage = message.Split(new char[] { '>' });
					if (splitMessage.Length > 3) {
						string ip = splitMessage[1];
						int port = int.Parse(splitMessage[2]);
						Debug.Log("Received ip: " + ip + ", port: " + port + " from " + serverData.RemoteEndPoint);
						string uniqueId = splitMessage[3];
						IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
						if (!encounteredIPs.Contains(endpoint)) {
							Debug.Log("Connecting to " + endpoint.Address + ", port " + endpoint.Port + "...");

							//try within different thread to connect - we'll allow another try within 10 seconds in case it does turn out to fail
							tcpClient.ConnectToHost(endpoint, uniqueId, serverData.RemoteEndPoint);
							encounteredIPs.Add(endpoint);
						}
					}
				}
			} catch(SocketException se) {
				Debug.LogError("Socket Exception (" + se.ErrorCode + "), cannot receive client data from host: " + se.ToString());
			} catch(Exception e) {
				Debug.Log("Error, cannot receive client data from host: " + e.ToString());
			}
		}
    }

	public async Task SendUDPMessage(IPEndPoint remote, byte[] data) {
		for (int i = 0; i < 2; ++i) {//send the message twice to be certain it reaches.
			try {
				await client.SendAsync(data, data.Length, remote);
			} catch(SocketException se) {
				Debug.LogWarning("[UDPListener] Socket error " + se.ErrorCode + ", cannot send UDP packet: " + se.ToString());
			} catch(Exception e) {
				Debug.LogWarning("[UDPListener] Error, cannot send UDP packet: " + e.ToString());
			}
		}
	}

	public void RemoveEncounteredIP(IPEndPoint endpoint) {
		encounteredIPs.Remove(endpoint);
	}

	private void OnDestroy() {
		Instance = null;
		client.Close();
		client = null;
	}
}
