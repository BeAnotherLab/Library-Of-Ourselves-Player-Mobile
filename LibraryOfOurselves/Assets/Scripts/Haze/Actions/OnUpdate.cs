using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnUpdate : MonoBehaviour {
	
	[SerializeField] bool mobile = false;
	[SerializeField] UnityEvent onUpdate;
	
	#if UNITY_EDITOR
	void Start(){
		if(mobile) mobile = !!mobile;//get rid of warning in editor :)
	}
	#endif
	
	void Update () {
		#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		if(mobile) onUpdate.Invoke();
		#else
		onUpdate.Invoke();
		#endif
	}
}
