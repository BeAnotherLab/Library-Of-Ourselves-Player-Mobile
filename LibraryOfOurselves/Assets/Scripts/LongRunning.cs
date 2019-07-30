using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRunning : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID
		UnityEngine.Android.AndroidDevice.SetSustainedPerformanceMode(true);
#endif
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
}
