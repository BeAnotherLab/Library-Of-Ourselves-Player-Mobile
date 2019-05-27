using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine.XR;

public class TCPClient : MonoBehaviour{

	[SerializeField] TCPConnection.DeviceType deviceType = TCPConnection.DeviceType.VR;
	[SerializeField] NewConnectionEvent onNewConnection;
	[SerializeField] MessageReceivedEvent onMessageReception;
	[SerializeField] ConnectionEndEvent onConnectionEnd;

	List<TCPConnection> hosts = new List<TCPConnection>();

	public static TCPClient Instance { get; private set; }

	public TCPConnection.DeviceType DeviceType {
		get { return deviceType; }
	}

	public string LockedId {
		get {
			if(HazePrefs.HasKey("locked-id"))
				return HazePrefs.GetString("locked-id");
			else return "free";
		}
		set {
			HazePrefs.SetString("locked-id", value);
			HazePrefs.Save();
		}
	}

	public bool HasAtLeastOneActiveConnection {
		get {
			return hosts.Count > 0;
		}
	}



	private void Start() {
		Instance = this;
	}

	public async Task ConnectToHost(IPEndPoint endpoint, string uniqueId) {
		TCPConnection connection = new TCPConnection();
		connection.uniqueId = uniqueId;
		connection.deviceType = TCPConnection.DeviceType.GUIDE;
		connection.client = new TcpClient();
		connection.client.NoDelay = true;
		IPAddress ipv4 = endpoint.Address;
		if(ipv4.AddressFamily != AddressFamily.InterNetwork)
			ipv4 = ipv4.MapToIPv4();
		Debug.Log("IPv4: " + ipv4);
		await connection.client.ConnectAsync(ipv4, endpoint.Port);
		Debug.Log("Connected. Sending identification message...");

		List<byte> data = new List<byte>();
		data.WriteString("identification");
		data.WriteByte((byte)deviceType);
		data.WriteString(SystemInfo.deviceUniqueIdentifier);
		data.WriteString(LockedId);
		data.WriteString(XRDevice.model);

		await connection.Send(data);
		hosts.Add(connection);

		onNewConnection.Invoke(connection);

		Communicate(connection);
	}

	private async void Communicate(TCPConnection connection) {
		while(connection.active) {
			List<byte> data = await connection.Receive();
			string channel = data.ReadString();
			if(channel == "disconnection") {
				connection.active = false;
			} else {
				//received a message from the host!
				onMessageReception.Invoke(connection, channel, data);
			}
		}
		onConnectionEnd.Invoke(connection);
		hosts.Remove(connection);
	}

	public void BroadcastToAllGuides(List<byte> data) {
		foreach(TCPConnection conn in hosts) {
			if(conn.active && conn.deviceType == TCPConnection.DeviceType.GUIDE) {
				conn.Send(data);
			}
		}
	}

	private void OnDestroy() {
		Instance = null;
		foreach(TCPConnection conn in hosts) {
			if(conn.active) {
				List<byte> data = new List<byte>();
				data.WriteString("disconnection");
				conn.Send(data);
			}
		}
	}

}
