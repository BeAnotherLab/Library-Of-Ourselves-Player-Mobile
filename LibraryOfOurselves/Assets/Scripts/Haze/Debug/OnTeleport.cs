using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTeleport : MonoBehaviour {
	
	[SerializeField] float minDist = 1.0f;//under this displacement /s, won't consider as teleported.
	[SerializeField] bool everyFrame = true;
	
	Vector3 position;
	
	void Start () {
		position = transform.position;
	}
	
	void Update () {
		if(everyFrame && checkIfTeleportNow())
			Debug.Log(Time.time+"    TELEPORTED");
	}
	
	public bool checkIfTeleportNow(){
		Vector3 newPos = transform.position;
		float magnitude = (newPos - position).magnitude;
		position = newPos;
		return magnitude > minDist*Time.deltaTime;
	}
}
