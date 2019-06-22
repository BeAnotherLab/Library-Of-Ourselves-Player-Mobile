using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnBuildTarget : MonoBehaviour{

	[SerializeField] UnityEvent onStandalone;
	[SerializeField] UnityEvent onAndroid;
	[SerializeField] UnityEvent onIos;

	private void Awake() {
#if UNITY_ANDROID
		onAndroid.Invoke();
#elif UNITY_IOS
		onIos.Invoke();
#else
		onStandalone.Invoke();
#endif
	}

}
