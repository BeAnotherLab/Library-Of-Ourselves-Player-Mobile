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

///Place this component on your camera (or any other GameObject you want to shake)
namespace Haze {
	public class Shake : MonoBehaviour {
		
		[SerializeField] bool onEnable = false;
		[SerializeField] [Range(0, 5)] float intensity = 1;
		[SerializeField] [Range(0, 50)] float angularIntensity = 10;
		[SerializeField] float duration = 0.3f;
		[SerializeField] [Range(0.0001f, 0.15f)] float damping = 0.05f;
		[SerializeField] bool useFixedDeltaTime = false;
		[SerializeField] Easing easing;
		[SerializeField] UnityEvent onEnd;
		
		///
		/// Properties
		///
		
		public float Intensity{
			get{ return intensity; }
			set{ intensity = value; }
		}
		
		public float AngularIntensity{
			get{ return angularIntensity; }
			set{ angularIntensity = value; }
		}
		
		public float Duration{
			get{ return duration; }
			set{ duration = value; }
		}
		
		///
		/// Functions
		///
		
		// -1..1
		static float noise(float seedX, float seedY, float elapsed){
			return Mathf.PerlinNoise(seedX + elapsed, seedY) * 2 - 1;
		}
		
		void OnEnable(){
			if(onEnable) Play();
		}
		
		void OnDisable(){
			Stop();
		}
		
		public void Play(){
			Stop();
			StartCoroutine(play(transform, intensity, angularIntensity, duration, easing, useFixedDeltaTime, damping, onEnd));
		}
		
		public void Stop(){
			StopAllCoroutines();
		}
		
		public static IEnumerator play(Transform t, float intensity = 1, float angularIntensity = 10, float duration = 0.3f, Easing easing = null, bool useFixedDeltaTime = false, float damping = 0.05f, UnityEvent onEnd = null){
			if(easing == null) easing = Easing.Linear;
			if(t == null) t = Camera.main.transform;
			
			// cache
			Vector3 originPosition = t.localPosition;
			Vector3 originRotation = t.localEulerAngles;
			float oneOverTime = 1.0f/duration;
			float oneOverDamping = 1.0f/damping;
			float seedX0 = Random.Range(0.0f, 100000.0f);
			float seedX1 = Random.Range(0.0f, 100000.0f);
			float seedY0 = Random.Range(0.0f, 100000.0f);
			float seedY1 = Random.Range(0.0f, 100000.0f);
			float seedZ0 = Random.Range(0.0f, 100000.0f);
			float seedZ1 = Random.Range(0.0f, 100000.0f);
			
			for(float elapsed = 0; elapsed < duration; elapsed += (useFixedDeltaTime ? Time.fixedDeltaTime : Time.deltaTime)){
				easing.T = elapsed * oneOverTime;
				float amount = 1 - easing.T;// 1..0
				
				t.localPosition = originPosition + new Vector3(noise(seedX0, seedX1, elapsed * oneOverDamping), noise(seedY0, seedY1, elapsed * oneOverDamping), noise(seedZ0, seedZ1, elapsed * oneOverDamping)) * amount * intensity;
				t.localEulerAngles = originRotation;
				t.Rotate(new Vector3(noise(seedX1, seedX0, elapsed * oneOverDamping), noise(seedY1, seedY0, elapsed * oneOverDamping), noise(seedZ1, seedZ0, elapsed * oneOverDamping)) * amount * angularIntensity);
				
				yield return null;
			}
			
			t.localPosition = originPosition;
			t.localEulerAngles = originRotation;
			
			if(onEnd != null) onEnd.Invoke();
			
		}
		public static IEnumerator play(float intensity = 1, float angularIntensity = 10, float duration = 0.3f, Easing easing = null, bool useFixedDeltaTime = false, float damping = 0.05f, UnityEvent onEnd = null){
			yield return play(null, intensity, angularIntensity, duration, easing, useFixedDeltaTime, damping, onEnd);
		}
		
	}
	
#if UNITY_EDITOR
	[CustomEditor(typeof(Shake))]
	public class ShakeEditor : Editor {
		public override void OnInspectorGUI(){
			
			DrawDefaultInspector();
			
			Shake shake = target as Shake;
			if(shake){
				
				//Preview in editor (play mode)
				if(EditorApplication.isPlaying){
					if(GUILayout.Button("Preview")){
						shake.Play();
					}
				}
				
			}
		}
	}
#endif

}