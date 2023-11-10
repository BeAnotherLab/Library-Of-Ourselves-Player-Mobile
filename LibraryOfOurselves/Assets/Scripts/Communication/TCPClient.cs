using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.XR;
using System.Linq;

public class TCPClient : MonoBehaviour{

	[SerializeField] TCPConnection.DeviceType deviceType = TCPConnection.DeviceType.VR;
	[SerializeField] NewConnectionEvent onNewConnection;
	[SerializeField] MessageReceivedEvent onMessageReception;
	[SerializeField] ConnectionEndEvent onConnectionEnd;
	[SerializeField] bool allowUdp = true;//allow setting up a UDP connection when TCP isn't available

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

	// If we're connected over UDP to the remote endpoint specified, returns true and reads the data.
	public bool ReceiveFakeTCPMessage(IPEndPoint udpRemote, byte[] data) {
		foreach(TCPConnection host in hosts) {
			if(host.UDP && sameEndpoint(host.udpEndpoint, udpRemote)) {
				host.ReceiveUDPPacket(data.ToList());
				return true;
			}
		}
		return false;
	}

	bool sameEndpoint(IPEndPoint one, IPEndPoint two) {
		if(one.Port != two.Port) return false;
		byte[] bytesOne = one.Address.GetAddressBytes();
		byte[] bytesTwo = two.Address.GetAddressBytes();
		if(bytesOne.Length == bytesTwo.Length) {
			for(int i = 0; i<bytesOne.Length; ++i) {
				if(bytesOne[i] != bytesTwo[i]) {
					return false;
				}
			}
			//got here means all the bytes are the same!
			return true;
		}
		return false;
	}

	public async Task<bool> ConnectToHost(IPEndPoint endpoint, string uniqueId, IPEndPoint udpEndpoint) {

		TCPConnection connection = new TCPConnection();
		connection.uniqueId = uniqueId;
		connection.deviceType = TCPConnection.DeviceType.GUIDE;
		connection.client = new TcpClient();
		connection.client.NoDelay = true;
		connection.sourceEndpoint = endpoint;
		IPAddress ipv4 = endpoint.Address;
		if(ipv4.AddressFamily != AddressFamily.InterNetwork)
			ipv4 = ipv4.MapToIPv4();
		Haze.Logger.Log("IPv4: " + ipv4);

		bool tcp = false;

		try {
			CancellationTokenSource cts = new CancellationTokenSource();
			cts.CancelAfter(10000);//Cancel after 10 seconds
			TaskCompletionSource<bool> cancellationCompletionSource = new TaskCompletionSource<bool>();

			var connectAsync = connection.client.Client.ConnectAsync(ipv4, endpoint.Port);

			using(cts.Token.Register(() => cancellationCompletionSource.TrySetResult(true))) {
				if(connectAsync != await Task.WhenAny(connectAsync, cancellationCompletionSource.Task)) {
					throw new OperationCanceledException(cts.Token);
				}
			}

			Haze.Logger.Log("Connected through TCP. Sending identification message...");
			tcp = true;
		} catch(SocketException se) {
			Haze.Logger.LogError("[TCPClient] Socket Exception (" + se.ErrorCode + "), cannot connect to host: " + se.ToString(), this);
		} catch(Exception e) {
			Haze.Logger.LogError("[TCPClient] Error, cannot connect to host: " + e.ToString(), this);
		}

		if(!tcp) {
			if(!allowUdp) return false;//nope, not allowed...
			//Something went wrong and TCP was unavailable. Fallback to UDP
			Haze.Logger.Log("Attempting to fall back to UDP with " + udpEndpoint);
			connection.client = null;
			connection.udpEndpoint = udpEndpoint;
		}

		try {
			List<byte> data = new List<byte>();
			data.WriteString("identification");
			data.WriteByte((byte)deviceType);
			data.WriteString(SystemInfo.deviceUniqueIdentifier);
			data.WriteString(LockedId);
			//data.WriteString(XRDevice.model);
			data.WriteString("Oculus Quest");

			await connection.Send(data);
			hosts.Add(connection);

			onNewConnection.Invoke(connection);
			Communicate(connection);

			return true;
		} catch(SocketException se) {
			Haze.Logger.LogError("[TCPClient] Socket Exception (" + se.ErrorCode + "), cannot send identification message to host: " + se.ToString(), this);
		} catch(Exception e) {
			Haze.Logger.LogError("[TCPClient] Error, cannot send identification message to host: " + e.ToString(), this);
		}

		UDPListener.Instance.RemoveEncounteredIP(endpoint);
		return false;
	}

	private async void Communicate(TCPConnection connection) {
		try {
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
		}catch(SocketException se) {
			Haze.Logger.LogError("[TCPClient] Socket Exception (" + se.ErrorCode + "), cannot communicate with host: " + se.ToString(), this);
		}catch(Exception e) {
			Haze.Logger.LogError("[TCPClient] Error, cannot communicate with host: " + e.ToString(), this);
		}
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
