using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSender : MonoBehaviour {
	
	[SerializeField] Transform cameraTransform;
	[SerializeField] Sender sender;
	[SerializeField] bool everyFrame = false;
	[SerializeField] float frequency = 0.2f;
	
	Transform t;
	
	void sendRot(){
		Quaternion rotationDelta = Quaternion.FromToRotation(cameraTransform.forward, t.forward);
		Vector3 angles = rotationDelta.eulerAngles;
		string data = angles.x + " " + angles.y + " " + angles.z;
		sender.Send(data);
	}
	
	void OnEnable(){
		t = transform;
		
		if(!everyFrame) StartCoroutine(sendingRoutine());
	}
	
	IEnumerator sendingRoutine(){
		while(true){
			sendRot();
			yield return new WaitForSeconds(frequency);
		}
	}
	
	void Update(){
		if(everyFrame) sendRot();
	}
	
}