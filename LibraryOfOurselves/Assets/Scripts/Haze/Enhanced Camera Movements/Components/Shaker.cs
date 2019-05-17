/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Enhanced Camera Movements".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

///Place this component anywhere in your scene. Use the DoShake() function to execute its shake on the main camera.
namespace Haze{
	public class Shaker : MonoBehaviour {
		
		[SerializeField] bool onEnable = false;
		[SerializeField] float delay = 0;
		[SerializeField] Transform toShake = null;//Main camera by default
		[SerializeField] [Range(0, 5)] float intensity = 1;
		[SerializeField] [Range(0, 50)] float angularIntensity = 10;
		[SerializeField] float duration = 0.3f;
		[SerializeField] [Range(0.0001f, 0.15f)] float damping = 0.05f;
		[SerializeField] bool useFixedDeltaTime = false;
		[SerializeField] Easing easing;
		[SerializeField] UnityEvent onEnd;
		
		void OnEnable(){
			if(onEnable) DoShake();
		}
		
		public void DoShake(){
			StartCoroutine(shake());
		}
		
		IEnumerator shake(){
			if(delay > 0)
				yield return new WaitForSeconds(delay);
			yield return Shake.play(toShake, intensity, angularIntensity, duration, easing, useFixedDeltaTime, damping, onEnd);
		}
		
	}
	
#if UNITY_EDITOR
	[CustomEditor(typeof(Shaker))]
	public class ShakerEditor : Editor {
		public override void OnInspectorGUI(){
			
			DrawDefaultInspector();
		}
	}
#endif

}