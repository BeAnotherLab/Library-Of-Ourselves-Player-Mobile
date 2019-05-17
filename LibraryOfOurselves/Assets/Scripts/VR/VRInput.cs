using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRInput : MonoBehaviour {

	[SerializeField] List<VRAction> actions;

	[System.Serializable]
	public class VRAction {
		public OVRInput.Button oculusGoInput;
		public GvrControllerButton mirageInput;
		public UnityEvent onInput;
	}

	void Update() {
		if(VRDevice.OculusGo) {//OculusGo
			OVRInput.Update();
			foreach(VRAction action in actions) {
				if(OVRInput.Get(action.oculusGoInput)) {
					action.onInput.Invoke();
				}
			}
		} else if(VRDevice.MirageSolo) {//MirageSolo
			foreach(VRAction action in actions) {
				if(GvrControllerInput.GetDevice(GvrControllerHand.Dominant).GetButton(action.mirageInput)) {
					action.onInput.Invoke();
				}
			}
		} else if(VRDevice.GearVR) {//GearVR (no support for actions)

		}
	}
}
