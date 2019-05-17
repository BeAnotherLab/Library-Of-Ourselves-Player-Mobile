using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orphan : MonoBehaviour {
	
	[SerializeField] bool followPosition = false;
	[SerializeField] bool followRotation = false;
    [SerializeField] bool followPitch = false;
    [SerializeField] bool followYaw = false;
    [SerializeField] bool followRoll = false;
    [SerializeField] float addPitch = 0;
    [SerializeField] float addYaw = 0;
    [SerializeField] float addRoll = 0;
    [SerializeField] Transform dad;
	
	Transform t;
    Vector3 angles;
	
	void Start(){
		t = transform;
        angles = t.eulerAngles;
		
		if(!followPosition && !followRotation && !followPitch && !followYaw && !followRoll){
			Debug.LogWarning("This orphan does not follow anything. Delete it.");
		}
		
		if(dad == null){
			Debug.LogWarning("This orphan has no dad. Ensure all orphans have a dad.");
		}
		
	}
	
	void LateUpdate(){
		
		if(followPosition && dad){
			t.position = dad.position;
		}else if((followPitch || followYaw || followRoll) && dad){
            Vector3 euler = dad.eulerAngles;
            Vector3 newAngles = angles;
            if (followPitch) newAngles.x = euler.x + addPitch;
            if (followYaw) newAngles.y = euler.y + addYaw;
            if (followRoll) newAngles.z = euler.z + addRoll;
            t.eulerAngles = newAngles;
        }
		
		if(followRotation && dad){
			t.rotation = dad.rotation;
		}
		
	}
	
}
