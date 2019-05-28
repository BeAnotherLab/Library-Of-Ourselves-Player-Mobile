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

	async void Start(){
		client = new UdpClient(listeningPort);
		while(client != null) {
			try {
				UdpReceiveResult serverData = await client.ReceiveAsync();
				string message = Encoding.ASCII.GetString(serverData.Buffer);
				//Extract IP and port from message
				string[] splitMessage = message.Split(new char[] { '>' });
				if(splitMessage.Length > 3) {
					string ip = splitMessage[1];
					int port = int.Parse(splitMessage[2]);
					string uniqueId = splitMessage[3];
					IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
					if(!encounteredIPs.Contains(endpoint)) {
						encounteredIPs.Add(endpoint);
						Debug.Log("Connecting to " + endpoint.Address + ", port " + endpoint.Port);
						//Start connection with this guide
						await tcpClient.ConnectToHost(endpoint, uniqueId);
					}
				}
			}catch(SocketException se) {
				Debug.LogError("Socket Exception (" + se.ErrorCode + "), cannot receive client data from host: " + se.ToString());
			}catch(Exception e) {
				Debug.Log("Error, cannot receive client data from host: " + e.ToString());
			}
		}
    }

	private void OnDestroy() {
		client.Close();
		client = null;
	}
}
