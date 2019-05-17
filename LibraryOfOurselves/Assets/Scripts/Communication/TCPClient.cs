using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TCPClient : MonoBehaviour{

	List<TCPConnection> hosts = new List<TCPConnection>();
    
	public async Task ConnectToHost(IPEndPoint endpoint, string uniqueId) {
		TCPConnection connection = new TCPConnection();
		connection.uniqueId = uniqueId;
		connection.client = new TcpClient();
		IPAddress ipv4 = endpoint.Address;
		if(ipv4.AddressFamily != AddressFamily.InterNetwork)
			ipv4 = ipv4.MapToIPv4();
		Debug.Log("IPv4: " + ipv4);
		await connection.client.ConnectAsync(ipv4, endpoint.Port);
		Debug.Log("Connected. Sending data...");
		connection.stream = connection.client.GetStream();
		List<byte> data = new List<byte>();
		data.WriteString("Hello there!");
		await connection.Send(data);
		hosts.Add(connection);
	}

}
