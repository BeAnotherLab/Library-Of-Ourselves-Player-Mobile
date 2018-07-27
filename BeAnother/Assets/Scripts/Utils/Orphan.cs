using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orphan : MonoBehaviour {
	
	[SerializeField] bool followPosition = false;
	[SerializeField] bool followRotation = false;
	[SerializeField] Transform dad;
	
	Transform t;
	
	void Start(){
		t = transform;
		
		if(!followPosition && !followRotation){
			Debug.LogWarning("This orphan does not follow anything. Delete it.");
		}
		
		if(dad == null){
			Debug.LogWarning("This orphan has no dad. Ensure all orphans have a dad.");
		}
		
	}
	
	void LateUpdate(){
		
		if(followPosition){
			t.position = dad.position;
		}
		
		if(followRotation){
			t.rotation = dad.rotation;
		}
		
	}
	
}
