using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour {
	
	[SerializeField] bool lockX = false;
	[SerializeField] bool lockY = false;
	[SerializeField] bool lockZ = false;
	[SerializeField] bool lockPitch = false;
	[SerializeField] bool lockYaw = false;
	[SerializeField] bool lockRoll = false;
	[SerializeField] bool lockRotation = false;
	
	[SerializeField] bool local = false;
	
	Transform t;
	
	Vector3 position;
	Vector3 angles;
	Quaternion rotation;
	
	void Start(){
		t = transform;
		
		position = getPosition();
		angles = getAngles();
		rotation = getRotation();
		
		if(lockRotation && (lockPitch || lockYaw || lockRoll))
			Debug.LogError("You cannot lock the euler angles and the rotation! Pick one.");
		else if(!lockX && !lockY && !lockZ && !lockPitch && !lockYaw && !lockRoll && !lockRotation)
			Debug.LogError("This lock is useless! Delete it.");
	}
	
	void LateUpdate(){
		if(lockX || lockY || lockZ){
			Vector3 pos = getPosition();
			if(lockX) pos.x = position.x;
			if(lockY) pos.y = position.y;
			if(lockZ) pos.z = position.z;
			setPosition(pos);
		}
		
		if(lockPitch || lockYaw || lockRoll){
			Vector3 ea = getAngles();
			if(lockPitch) ea.x = angles.x;
			if(lockYaw) ea.y = angles.y;
			if(lockRoll) ea.z = angles.z;
			setAngles(ea);
		}else if(lockRotation){
			setRotation(rotation);
		}
	}
	
	Vector3 getPosition(){
		return local? t.localPosition : t.position;
	}
	
	void setPosition(Vector3 pos){
		if(local) t.localPosition = pos;
		else t.position = pos;
	}
	
	Vector3 getAngles(){
		return local ? t.localEulerAngles : t.eulerAngles;
	}
	
	void setAngles(Vector3 angles){
		if(local) t.localEulerAngles = angles;
		else t.eulerAngles = angles;
	}
	
	Quaternion getRotation(){
		return local ? t.localRotation : t.rotation;
	}
	
	void setRotation(Quaternion rot){
		if(local) t.localRotation = rot;
		else t.rotation = rot;
	}
	
}
