using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRDevice{
	
	public static bool MirageSolo{
		get{
			#if (UNITY_ANDROID && !UNITY_EDITOR)
			return XRDevice.model.ToLower().Contains("mirage solo");
			#else
			return false;
			#endif
		}
	}
	
	public static bool OculusGo{
		get{
			#if (UNITY_ANDROID && !UNITY_EDITOR)
			return XRDevice.model.ToLower().Contains("oculus go");
			#else
			return false;
			#endif
		}
	}
	
	public static bool GearVR{
		get{
			#if (UNITY_ANDROID && !UNITY_EDITOR)
			return !OculusGo && !MirageSolo;
			#else
			return false;
			#endif
		}
	}
	
}
