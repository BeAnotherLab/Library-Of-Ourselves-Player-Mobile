using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class VRGazeChoice : MonoBehaviour {

	public delegate  void OnConfirmVRGazeSelection(int index);
	public static OnConfirmVRGazeSelection ConfirmVRGazeSelection;
	public int choiceIndex;
	
	[SerializeField] float maxDistanceFromCam = 100;
	[SerializeField] UnityEvent onStartGazing;
	[SerializeField] UnityEvent onStopGazing;
	[SerializeField] bool verbose = false;
	
	bool gazing = false;

	private void Update() {
		Camera cam = Camera.main;
		Ray ray = new Ray(cam.transform.position, cam.transform.forward);
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo, maxDistanceFromCam)) {
			if(hitInfo.collider == GetComponent<Collider>()) {
				turnGazeOn();//hit this
			} else {
				turnGazeOff();//hit something that wasn't this
			}
		} else {
			turnGazeOff();//didnt hit anything at all
		}
	}

	void turnGazeOn() {
		if(!gazing) {
			if(verbose) Haze.Logger.Log("Gazing at: " + name);
			onStartGazing.Invoke();
			gazing = true;
		}
	}

	void turnGazeOff() {
		if(gazing) {
			if(verbose) Haze.Logger.Log("No longer gazing at " + name);
			onStopGazing.Invoke();
			gazing = false;
		}
	}

	public void OnSelectionComplete()
	{
		ConfirmVRGazeSelection(choiceIndex);
	}
}
