using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitThen : MonoBehaviour {

	[SerializeField] float delay = 7.0f;
	[SerializeField] bool onStart = true;
	[SerializeField] UnityEvent onFinishWaiting;

	IEnumerator Start(){
		if (onStart) {

			yield return new WaitForSeconds (delay);
			onFinishWaiting.Invoke ();
		}

	}

	public void Run(){
		StartCoroutine (run ());
	}

	IEnumerator run(){
		yield return new WaitForSeconds (delay);
		onFinishWaiting.Invoke ();
	}
}
