using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseSpinner : MonoBehaviour {
	
	[SerializeField] float damp = 10;
	[SerializeField] LayerMask layers;
	[SerializeField] float smoothPointToPointSpeed = 100;
	[SerializeField] Interpolation interpolation = null;//optional, for smooth point to point rotations
	[SerializeField] UnityEvent onDragStart;
	
	Transform t = null;
	bool touching = false;
	bool dragStart = false;
	Vector3 previousPosition;
	Vector3 currentPosition;
	Vector3 axis;
	float angle = 0;
	
	UnityEvent doAfterPtoP = null;//will be called after the next point to point rotate
	
	void Start(){
		t = transform;
	}
	
	void Update(){
		
		if(Input.GetMouseButton(0)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, layers.value)){
				//touching the object at hit.point
				stopPointToPoint();
				if(!touching){//first frame
					touching = true;
					previousPosition = hit.point;
					currentPosition = hit.point;
					dragStart = true;
				}else{
					previousPosition = currentPosition;
					currentPosition = hit.point;
					
					if(dragStart && previousPosition != currentPosition){
						dragStart = false;
						onDragStart.Invoke();
					}
					computeRotation();
				}
			}else{
				//not touching the object, but clicking somewhere else
				angle = 0;
				touching = false;
			}
		}else{
			//not touching the object
			touching = false;
		}
		
		//apply the rotation
		if(angle > 0){
			if(!touching){
				//damp angle first
				angle = Mathf.MoveTowards(angle, 0, Time.deltaTime * damp);
			}
			t.RotateAround(t.position, axis, angle);
		}
		
	}
	
	//sets axis and angle
	void computeRotation(){
		if(previousPosition == currentPosition){ angle = 0; return; }//no need!
		
		Vector3 c = t.position;
		Vector3 ca = previousPosition - c;
		Vector3 cb = currentPosition - c;
		//figure out axis: perpendicular to both vectors [ca] and [cb]; passing through c
		axis = Vector3.Cross(ca, cb);
		angle = Vector3.SignedAngle(ca, cb, axis);
	}
	
	public void PointToPointRotate(Vector3 fromPosition, Vector3 toPosition){
		if(t == null)t = transform;
		previousPosition = fromPosition;
		currentPosition = toPosition;
		computeRotation();
		if(interpolation != null){//smooth mode
			StartCoroutine(pointToPoint(axis, angle));
		}else{//immediate mode
			t.RotateAround(t.position, axis, angle);
		}
		angle = 0;//stop immediately any other rotation
	}
	
	public void PointToPointRotate(Vector3 fromPosition, Vector3 toPosition, UnityEvent thenDo){
		doAfterPtoP = thenDo;
		PointToPointRotate(fromPosition, toPosition);
	}
	
	IEnumerator pointToPoint(Vector3 axis, float angle){
		Quaternion startRotation = t.rotation;
		t.RotateAround(t.position, axis, angle);
		Quaternion endRotation = t.rotation;
		t.rotation = startRotation;
		interpolation.Speed = smoothPointToPointSpeed/angle;
		interpolation.Interpolate();
		while(interpolation.Interpolating){
			t.rotation = Quaternion.Lerp(startRotation, endRotation, interpolation.Value);
			yield return null;
		}
		//done
		t.rotation = endRotation;
		if(doAfterPtoP != null){
			doAfterPtoP.Invoke();
			doAfterPtoP = null;
		}
	}
	
	void stopPointToPoint(){
		if(doAfterPtoP != null){
			doAfterPtoP.Invoke();
			doAfterPtoP = null;
		}
		StopAllCoroutines();
		interpolation.Stop();
	}
	
}
