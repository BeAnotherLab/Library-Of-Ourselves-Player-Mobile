using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour{
    
	public static Status Instance { get; set; }

	public int Battery {
		get {
			return 0;//TODO
		}
	}

	public float FPS {
		get {
			return 0;//TODO
		}
	}

	public int Temperature {
		get {
			return 0;//TODO
		}
	}


	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}

}
