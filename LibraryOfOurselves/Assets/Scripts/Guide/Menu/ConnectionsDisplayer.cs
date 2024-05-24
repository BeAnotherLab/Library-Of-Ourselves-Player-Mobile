using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionsDisplayer : MonoBehaviour //TODO this isn't doing any UI stuff apart from instantiating the display prefab and associate it to the connection. Could be in networking
{
	[SerializeField] private GameObject _connectionDisplayPrefab;

	public static ConnectionsDisplayer Instance;

	public class DisplayedConnectionHandle {
		public TCPConnection connection;
		public ConnectionDisplay display;
	}

	public List<DisplayedConnectionHandle> handles = new List<DisplayedConnectionHandle>();

	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}

	public void AddConnection(TCPConnection connection) {
		DisplayedConnectionHandle handle = new DisplayedConnectionHandle();
		handle.connection = connection;
		handle.display = Instantiate(_connectionDisplayPrefab, transform).GetComponent<ConnectionDisplay>();
		handle.display.Init(connection);
		handles.Add(handle);
	}

	public void RemoveConnection(TCPConnection connection) {
		DisplayedConnectionHandle handle = GetConnectionHandle(connection);
		if (handle != null) {
			Destroy(handle.display.gameObject);
			handles.Remove(handle);
		}
	}

	public DisplayedConnectionHandle GetConnectionHandle(TCPConnection connection) {
		foreach (DisplayedConnectionHandle handle in handles) {
			if (handle.connection == connection) return handle;
		}
		return null;
	}

}
