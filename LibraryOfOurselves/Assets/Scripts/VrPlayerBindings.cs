using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_ANDROID
public class  VrPlayerBindings{

	static VrPlayerBindings __instance = null;
	public static VrPlayerBindings Instance {
		get {
			if(__instance == null)
				__instance = new VrPlayerBindings();
			return __instance;
		}
	}

	AndroidJavaObject __ajo = null;
	AndroidJavaObject JavaObject {
		get {
#if UNITY_ANDROID && !UNITY_EDITOR
			if(__ajo == null) {
				try {
					AndroidJavaClass unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject currentActivity = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
					AndroidJavaClass vpbClass = new AndroidJavaClass("sco.haze.vpb.VrPlayerBindings");
					if(vpbClass != null) {
						__ajo = vpbClass.CallStatic<AndroidJavaObject>("getInstance", context);
					}
				}catch(Exception e) {
					Debug.LogError("Cannot access VrPlayerBindings native Android code.\n" + e);
				}
			}
#endif
			return __ajo;
		}
	}




	/////Bindings to the java code/////


	public float GetTemperature() {
		if(JavaObject != null) {
			return JavaObject.Call<float>("getTemperature");
		} else return float.NegativeInfinity;
	}
	
	public string GetSDCardPath() {
		if(JavaObject != null) {
			return JavaObject.Call<string>("getSDCardPath");
		} else return null;
	}

	public string GetMessage() {
		if(JavaObject != null) {
			return JavaObject.Get<string>("message");
		} else return "Message unavailable.";
	}

	public bool isExternalStoragePermissionEnabled() {
		if(JavaObject != null) {
			return JavaObject.Call<bool>("isExternalStoragePermissionEnabled");
		} else return true;//Assume it is
	}

	public void requestExternalStoragePermission(GameObject callbackObject, string callbackMethodName) {
		if(JavaObject != null) {
			JavaObject.Call("requestExternalStoragePermission", callbackObject.name, callbackMethodName);
		}
	}

}
#endif