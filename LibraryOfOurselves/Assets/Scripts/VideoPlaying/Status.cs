using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour{
    
	public static Status Instance { get; set; }

	public int Battery {
		get {
			return (int) (SystemInfo.batteryLevel * 100);
		}
	}

	int framesCounted = 0;
	float elapsedSinceFPSEpoch = 0;
	public float FPS {
		get; private set;
	}

	public int Temperature {
		get {
			return 50;
		}
	}

	private void Start() {
		Instance = this;
		FPS = 60;
	}

	private void OnDestroy() {
		Instance = null;
	}

	private void Update() {
		elapsedSinceFPSEpoch += Time.deltaTime;
		++framesCounted;
		if(elapsedSinceFPSEpoch > 0.5f) {
			FPS = (float)framesCounted * 2;
			elapsedSinceFPSEpoch -= 0.5f;
			framesCounted = 0;
		}
	}

}
