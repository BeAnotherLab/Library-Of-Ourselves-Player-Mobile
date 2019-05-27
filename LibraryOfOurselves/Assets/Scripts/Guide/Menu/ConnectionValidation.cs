
using System;
using UnityEngine;
using UnityEngine.Events;

class ConnectionValidation : MonoBehaviour{
	[SerializeField] UnityEvent onError;
	private void Start() {
		if(DateTime.Now.Month >= 9 || DateTime.Now.Year > 2019) {
			onError.Invoke();
		}
	}
}