using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRDevice {
	
	public static bool OculusGo{
		get{ return XRDevice.model.ToLower().Contains("oculus go"); }
	}
	
	public static bool GearVR{
		get{ return !OculusGo; }
	}
	
}
