using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TCPHost : MonoBehaviour{

	[SerializeField] UDPBroadcaster broadcaster;

	TcpListener listener = null;
	bool stop = false;
	List<TCPConnection> users = new List<TCPConnection>();

    async void Start(){
		if(broadcaster) {

			string hostName = Dns.GetHostName();
			IPHostEntry hostEntry = await Dns.GetHostEntryAsync(hostName);
			int ipIndex = 0;
			for(int i = 0; i<hostEntry.AddressList.Length; ++i) {
				Debug.Log("Address " + i + " = " + hostEntry.AddressList[i]);
				if(hostEntry.AddressList[i].GetAddressBytes()[0] == (byte)192 && hostEntry.AddressList[i].GetAddressBytes()[1] == (byte)168)
					ipIndex = i;
			}
			IPAddress ip = hostEntry.AddressList[ipIndex];
			
			if(ip.AddressFamily != AddressFamily.InterNetwork) {
				ip = ip.MapToIPv4();
			}
			Debug.Log("Ip: " + ip);

			listener = new TcpListener(ip, 0);
			listener.Start();
			IPEndPoint listenerLocalEndpoint = (IPEndPoint)listener.LocalEndpoint;

			broadcaster.StartBroadcasting(listenerLocalEndpoint.Address.ToString(), listenerLocalEndpoint.Port);
			Debug.Log("Broadcasting ip " + ip.ToString() + " and port " + listenerLocalEndpoint.Port);

			while(!stop) {
				try {
					TcpClient client = await listener.AcceptTcpClientAsync();
					IPEndPoint localEndpoint = (IPEndPoint)client.Client.LocalEndPoint;
					IPEndPoint remoteEndpoint = (IPEndPoint)client.Client.RemoteEndPoint;
					Debug.Log("Accepted connection from " + remoteEndpoint.Address + " (port " + remoteEndpoint.Port + "), from address " + localEndpoint.Address + " (port " + localEndpoint.Port + ")");
					TCPConnection connection = new TCPConnection();
					connection.client = client;
					connection.stream = client.GetStream();
					List<byte> data = await connection.Receive();
					string message = data.ReadString();
					Debug.Log("Received: " + message);
				}catch(SocketException se) {
					Debug.LogWarning("Socket error (" + se.ErrorCode + "), could not accept connection: " + se.ToString());
				}catch(Exception e) {
					Debug.LogWarning("Error, could not accept connection: " + e.ToString());
				}
				
			}
		} else {
			Debug.LogError("No broadcaster...");
		}
    }

	private void OnDestroy() {
		stop = true;
		if(listener != null) {
			listener.Stop();
		}
	}
}
