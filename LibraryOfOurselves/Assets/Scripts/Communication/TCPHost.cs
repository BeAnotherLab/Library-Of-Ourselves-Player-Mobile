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
	[SerializeField] TCPConnection.DeviceType deviceType = TCPConnection.DeviceType.GUIDE;
	[SerializeField] NewConnectionEvent onNewConnection;
	[SerializeField] MessageReceivedEvent onMessageReception;
	[SerializeField] ConnectionEndEvent onConnectionEnd;
	[SerializeField] float unresponsiveThreshold = 2;//After this amount of time, device will be shown as unresponsive
	[SerializeField] ResponsivenessEvent onResponsivenessChanged;

	public static TCPHost Instance { get; private set; }

	TcpListener listener = null;
	bool stop = false;
	List<TCPConnection> users = new List<TCPConnection>();

	public int NumberOfPairedDevices {
		get {
			int i = 0;
			foreach(TCPConnection conn in users) {
				if(conn.active && conn.paired)
					++i;
			}
			return i;
		}
	}

    async void Start() {
		Instance = this;
		if(broadcaster) {

			string hostName = Dns.GetHostName();
			IPHostEntry hostEntry = await Dns.GetHostEntryAsync(hostName);
			int ipIndex = -1;
			for(int i = 0; i<hostEntry.AddressList.Length; ++i) {
				IPAddress thisOne = hostEntry.AddressList[i];
				if(thisOne.AddressFamily != AddressFamily.InterNetwork && thisOne.IsIPv4MappedToIPv6) {
					thisOne = thisOne.MapToIPv4();
				}
				Debug.Log("Address " + i + " = " + thisOne);
				if(thisOne.GetAddressBytes()[0] == (byte)192 && thisOne.GetAddressBytes()[1] == (byte)168) {
					ipIndex = i;
					break;
				}
			}
			IPAddress ip;
			if(ipIndex > -1)
				ip = hostEntry.AddressList[ipIndex];
			else
				ip = IPAddress.Any;
			
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

					Debug.Log("Awaiting a connection request...");
					TcpClient client = await listener.AcceptTcpClientAsync();
					client.NoDelay = true;
					IPEndPoint localEndpoint = (IPEndPoint)client.Client.LocalEndPoint;
					IPEndPoint remoteEndpoint = (IPEndPoint)client.Client.RemoteEndPoint;
					Debug.Log("Accepted connection from " + remoteEndpoint.Address + " (port " + remoteEndpoint.Port + "), from address " + localEndpoint.Address + " (port " + localEndpoint.Port + ")");
					TCPConnection connection = new TCPConnection();
					connection.client = client;

					//wait for identification message:
					List<byte> data = await connection.Receive();
					string channel = data.ReadString();
					if(channel != "identification") {
						Debug.LogWarning("Device at " + remoteEndpoint.Address + " has responded with an illegal channel: " + channel);
					} else {
						connection.deviceType = (TCPConnection.DeviceType)data.ReadByte();
						connection.uniqueId = data.ReadString();
						connection.lockedId = data.ReadString();
						connection.xrDeviceModel = data.ReadString();

						users.Add(connection);

						onNewConnection.Invoke(connection);

						Communicate(connection);
					}

				}catch(SocketException se) {
					Debug.LogWarning("Socket error (" + se.ErrorCode + "), could not accept connection: " + se.ToString());
					Debug.LogWarning("Attempting to restart TCPHost::Start()");
					listener.Stop();
					listener = null;
					Start();//retry
					return;
				}catch(Exception e) {
					if(listener != null)
						Debug.LogWarning("Error, could not accept connection: " + e.ToString());
					//else just means that we're exiting Unity.
				}
				
			}
		} else {
			Debug.LogError("No broadcaster...");
		}
    }

	private void Update() {
		foreach(TCPConnection conn in users) {
			if(conn.active) {
				if(conn.responsive && conn.TimeSinceLastConnection > unresponsiveThreshold) {
					conn.responsive = false;
					onResponsivenessChanged.Invoke(conn, false);
				} else if(!conn.responsive && conn.TimeSinceLastConnection < unresponsiveThreshold) {
					conn.responsive = true;
					onResponsivenessChanged.Invoke(conn, true);
				}
			}
		}
	}

	public void ReceiveUDPPacket(IPEndPoint remote, List<byte> data) {
		foreach(TCPConnection conn in users) {//is this addressed to one of our local connections?
			if(conn.UDP) {
				conn.ReceiveUDPPacket(data);
				return;
			}
		}
		//it hasn't found a receiver, if this is an identification message then we can start a connection with them over udp
		//wait for identification message:
		string channel = data.ReadString();
		if(channel == "identification") {
			Debug.Log("Accepted [UDP] connection from " + remote.Address + " (port " + remote.Port + ")");

			TCPConnection connection = new TCPConnection();
			connection.udpEndpoint = remote;//set it up as UDP

			connection.deviceType = (TCPConnection.DeviceType)data.ReadByte();
			connection.uniqueId = data.ReadString();
			connection.lockedId = data.ReadString();
			connection.xrDeviceModel = data.ReadString();

			users.Add(connection);

			onNewConnection.Invoke(connection);

			Communicate(connection);
		}
	}

	private async void Communicate(TCPConnection connection) {
		while(connection.active) {
			List<byte> data = await connection.Receive();
			if(data.Count > 0) {
				string channel = data.ReadString();
				if(channel == "disconnection") {
					connection.active = false;
				} else {
					//received a message from the host!
					onMessageReception.Invoke(connection, channel, data);
				}
			} else {
				Debug.LogWarning("Received data with length == 0 from connection " + connection);
				connection.active = false;
			}
		}

		//Attempt to send a "disconnection" message before closing
		{
			List<byte> data = new List<byte>();
			data.WriteString("disconnection");
			try {
				await connection.Send(data);
			} catch(Exception e) {
				//nevermind.
			}
		}

		onConnectionEnd.Invoke(connection);
		users.Remove(connection);
		if(!connection.UDP) {
			connection.client.Close();
		}
	}

	public void BroadcastToPairedDevices(List<byte> data) {
		foreach(TCPConnection conn in users) {
			if(conn.active && conn.paired) {
				conn.Send(data);
			}
		}
	}

	private void OnDestroy() {

		Instance = null;

		foreach(TCPConnection conn in users) {
			if(conn.active) {
				List<byte> data = new List<byte>();
				data.WriteString("disconnection");
				conn.Send(data);
			}
		}

		stop = true;
		if(listener != null) {
			listener.Stop();
			listener = null;
		}
	}
}
