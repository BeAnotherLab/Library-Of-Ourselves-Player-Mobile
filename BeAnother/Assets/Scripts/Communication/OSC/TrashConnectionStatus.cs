using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashConnectionStatus : MonoBehaviour {
	
	public void Trash(){
		if(ConnectionStatus.Instance != null){
			ConnectionStatus.Instance.TrashObject();
		}
	}
	
}
