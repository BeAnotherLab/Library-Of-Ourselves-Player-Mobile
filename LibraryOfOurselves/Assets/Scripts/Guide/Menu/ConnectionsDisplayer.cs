using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionsDisplayer : MonoBehaviour{

	[SerializeField] GameObject connectionDisplayPrefab;

	public static ConnectionsDisplayer Instance { get; set; }

	public class DisplayedConnectionHandle {
		public TCPConnection connection;
		public ConnectionDisplay display;
	}

	List<DisplayedConnectionHandle> handles = new List<DisplayedConnectionHandle>();
	public List<DisplayedConnectionHandle> Handles { get { return handles; } }

	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}

	public void AddConnection(TCPConnection connection) {
		DisplayedConnectionHandle handle = new DisplayedConnectionHandle();
		handle.connection = connection;
		handle.display = Instantiate(connectionDisplayPrefab, transform).GetComponent<ConnectionDisplay>();
		handle.display.Init(connection);
		handles.Add(handle);
	}

	public void RemoveConnection(TCPConnection connection) {
		DisplayedConnectionHandle handle = GetConnectionHandle(connection);
		if(handle != null) {
			Destroy(handle.display.gameObject);
			handles.Remove(handle);
		}
	}

	public DisplayedConnectionHandle GetConnectionHandle(TCPConnection connection) {
		foreach(DisplayedConnectionHandle handle in handles) {
			if(handle.connection == connection)
				return handle;
		}
		return null;
	}

}
