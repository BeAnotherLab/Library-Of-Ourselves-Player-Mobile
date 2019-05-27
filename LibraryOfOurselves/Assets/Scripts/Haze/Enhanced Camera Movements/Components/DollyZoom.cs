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

///Place this component on your main camera to add dolly zooms to your scene
namespace Haze{
	[RequireComponent(typeof(Camera))]
	public class DollyZoom : MonoBehaviour {
		
		[SerializeField] bool onEnable = false;
		[SerializeField] Transform target;//what we will zoom onto/out of
		[SerializeField] bool useColliderDistance = false;//turn this off for performance
		[SerializeField] bool modifyDepthOfField = false;
		[SerializeField] bool lookAt = false;
		[SerializeField] bool infinite = false;//if infinite is true, Distance, Time, UseFixedDeltaTime, Easing and OnEnd will be ignored.
		[SerializeField] float distance = -10;//how much we want the camera to move in/out from subject
		[SerializeField] float time = 1;//how long the effect should take
		[SerializeField] bool useFixedDeltaTime = false;
		[SerializeField] Easing easing;
		[SerializeField] UnityEvent onEnd;
		
		///
		/// Properties
		///
		
		public Transform Target {
			get{ return target; }
			set{ target = value; }
		}
		
		public float Distance {
			get{ return distance; }
			set{ distance = value; }
		}
		
		public float Duration {
			get{ return time; }
			set{ time = value; }
		}
		
		public bool UseColliderDistance {
			get{ return useColliderDistance; }
			set{ useColliderDistance = value; }
		}
		
		public bool ModifyDepthOfField {
			get{ return modifyDepthOfField; }
			set{ modifyDepthOfField = value; }
		}
		
		public bool Look {
			get{ return lookAt; }
			set{ lookAt = value; }
		}
		
		///
		/// Functions
		///
		
		void OnEnable(){
			if(onEnable) Play();
		}
		
		void OnDisable(){
			Stop();
		}
		
		/// Calculate frustum height at given distance from camera
		static float frustumHeight(float distance, float fov){
			return 2.0f * distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
		}
		
		/// Calculate fov for a certain height at a certain distance
		static float fov(float height, float distance){
			return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
		}
		
		// Get distance to target
		static float distanceToTarget(Transform camT, Transform target, Collider targetColl = null){
			Vector3 targetPosition;//world space
			if(targetColl){//use the closest point on the collider if available (warning: less performance)
				targetPosition = targetColl.ClosestPointOnBounds(camT.position);
			}else{
				targetPosition = target.position;
			}
			targetPosition = camT.InverseTransformPoint(targetPosition);//local space to camera
			return Mathf.Abs(targetPosition.z);
		}
		
		/// Plays the dolly zoom effect according to the parameters set in this component.
		public void Play(){
			Stop();
			if(infinite)
				StartCoroutine(playNoMovement(GetComponent<Camera>(), target, useColliderDistance, modifyDepthOfField, lookAt));
			else
				StartCoroutine(play(GetComponent<Camera>(), target, distance, easing, time, useColliderDistance, useFixedDeltaTime, modifyDepthOfField, lookAt, onEnd));
		}
		
		/// Stops the dolly zoom effect
		public void Stop(){
			StopAllCoroutines();
		}
		
		/// Alternatively use this from a MonoBehaviour as: StartCoroutine(DollyZoom.play(Camera.main, ...));
		/// Or within another IEnumerator: yield return DollyZoom.play(Camera.main, ...);
		public static IEnumerator play(Camera cam, Transform target, float distance, Easing easing = null, float time = 1, bool useColliderDistance = false, bool useFixedDeltaTime = false, bool modifyDepthOfField = false, bool lookAt = false, UnityEvent onEnd = null){
			if(easing == null) easing = Easing.Linear;
			if(cam == null) cam = Camera.main;
			
			// checks
			if(!target){
				Debug.LogWarning("Cannot execute dolly zoom; target is null.");
				yield break;
			}
			
			// cache
			Transform camT = cam.transform;
			float oneOverTime = 1.0f/time;
			Vector3 initialPosition = camT.position;
			Collider targetColl = null;
			if(useColliderDistance) targetColl = target.GetComponent<Collider>();
			float initialDistance = distanceToTarget(camT, target, targetColl);
			float initialHeight = frustumHeight(initialDistance, cam.fieldOfView);
			
			for(float elapsed = 0; elapsed < time; elapsed += (useFixedDeltaTime ? Time.fixedDeltaTime : Time.deltaTime)){
				easing.T = elapsed * oneOverTime;
				//move camera in/out
				float distanceMoved = distance * easing.T;
				camT.position = camT.forward * distanceMoved + initialPosition;
				//get current distance to target
				float currentDistance = distanceToTarget(camT, target, targetColl);
				//modify fov
				cam.fieldOfView = fov(initialHeight, currentDistance);
				//set depth of field focus distance if we have post processing stack
				if(modifyDepthOfField)
					PostProcessingBridge.SetDepthOfField_FocusDistance(currentDistance, cam);
				if(lookAt)
					LookAt.Look(cam, target, 0, false, false);
				yield return null;
			}
			
			if(onEnd != null)
				onEnd.Invoke();
			
		}
		public static IEnumerator play(Transform target, float distance, Easing easing = null, float time = 1, bool useColliderDistance = false, bool useFixedDeltaTime = false, bool modifyDepthOfField = false, bool lookAt = false, UnityEvent onEnd = null){
			yield return play(null, target, distance, easing, time, useColliderDistance, useFixedDeltaTime, modifyDepthOfField, lookAt, onEnd);
		}
		
		/// Or, if you want to keep the effect while moving the camera yourself from somewhere else
		/// Use as StartCoroutine(DollyZoom.playNoMovement(Camera.main, ...);
		/// (Don't yield return DollyZoom.playNoMovement(Camera.main, ...) as you will not be able to stop it).
		public static IEnumerator playNoMovement(Camera cam, Transform target, bool useColliderDistance = false, bool modifyDepthOfField = false, bool lookAt = false){
			if(cam == null) cam = Camera.main;
			
			//checks
			if(!target){
				Debug.LogWarning("Cannot execute dolly zoom; target is null.");
				yield break;
			}
			
			// cache
			Transform camT = cam.transform;
			Collider targetColl = null;
			if(useColliderDistance) targetColl = target.GetComponent<Collider>();
			//get initial distance to target
			float initialDistance = distanceToTarget(camT, target, targetColl);
			float initialHeight = frustumHeight(initialDistance, cam.fieldOfView);
			
			while(true){
				//get current distance to target
				float currentDistance = distanceToTarget(camT, target, targetColl);
				//modify fov
				cam.fieldOfView = fov(initialHeight, currentDistance);
				//set depth of field focus distance if we have post processing stack
				if(modifyDepthOfField)
					PostProcessingBridge.SetDepthOfField_FocusDistance(currentDistance, cam);
				if(lookAt)
					LookAt.Look(cam, target, 0, false, false);
				yield return null;
			}
		}
		public static IEnumerator playNoMovement(Transform target, bool useColliderDistance = false, bool modifyDepthOfField = false, bool lookAt = false){
			yield return playNoMovement(null, target, useColliderDistance, modifyDepthOfField, lookAt);
		}
		
	}
	
#if UNITY_EDITOR
	[CustomEditor(typeof(DollyZoom))]
	public class DollyZoomEditor : Editor {
		public override void OnInspectorGUI(){
			
			DrawDefaultInspector();
			
			DollyZoom dollyZoom = target as DollyZoom;
			if(dollyZoom){
				
				//Preview in editor (play mode)
				if(EditorApplication.isPlaying){
					if(GUILayout.Button("Preview (single)")){
						dollyZoom.Play();
					}
					if(GUILayout.Button("Preview (back and forth)")){
						dollyZoom.Play();
						dollyZoom.Distance = -dollyZoom.Distance;//invert effect each step.
					}
				}
				
				//Show warnings if needed
				bool showWarnings = !EditorPrefs.HasKey("HazeWarnings") || EditorPrefs.GetBool("HazeWarnings") == true;
				bool warnings = false;//true if there's at least one warning.
				GUIStyle warningStyle = new GUIStyle();
				warningStyle.wordWrap = true;
				warningStyle.normal.textColor = new Color(1, 0.3f, 0);
				if(dollyZoom.ModifyDepthOfField){
					#if !HAZE_POSTPROCESSING
					if(showWarnings) EditorGUILayout.LabelField("Warning: depth of field won't be modified. If you use the Unity Post Processing Stack, you need to add HAZE_POSTPROCESSING to the Scripting Define Symbols in your project's Player Settings.", warningStyle);
					warnings = true;
					#endif
					if(!dollyZoom.UseColliderDistance){
						if(showWarnings) EditorGUILayout.LabelField("Warning: depth of field will work better if you enable Use Collider Distance (as long as the target has a Collider component attached).", warningStyle);
						warnings = true;
					}
					if(dollyZoom.GetComponent<Focus>() != null){
						if(showWarnings) EditorGUILayout.LabelField("Warning: Modify Depth Of Field is enabled but this camera already has a Focus component. Get rid of the Focus component, or disable Modify Depth Of Field.", warningStyle);
						warnings = true;
					}
				}else if(dollyZoom.UseColliderDistance){
					if(showWarnings) EditorGUILayout.LabelField("Warning: distance will be taken from collider, which will affect performance; you didn't enable Modify Depth Of Field, so you may want to disable Use Collider Distance as well.", warningStyle);
					warnings = true;
				}
				if(dollyZoom.Target == null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: Target is null.", warningStyle);
					warnings = true;
				}
				else if(dollyZoom.UseColliderDistance && dollyZoom.Target.GetComponent<Collider>() == null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: you enabled Use Collider Distance, however the target does not have a collider component attached.", warningStyle);
					warnings = true;
				}
				if(dollyZoom.GetComponent<LookAt>() != null){
					if(dollyZoom.Look){
						if(showWarnings) EditorGUILayout.LabelField("Warning: Look At is enabled but this camera already has a LookAt component. Get rid of the LookAt component, or disable Look At.", warningStyle);
						warnings = true;
					}
					if(dollyZoom.GetComponent<LookAt>().ModifyDepthOfField && dollyZoom.ModifyDepthOfField){
						if(showWarnings) EditorGUILayout.LabelField("Warning: Modify Depth Of Field is enabled on both this DollyZoom and on this camera's LookAt component; disable either. Alternatively get rid of the LookAt component and enable Look At on this DollyZoom component.", warningStyle);
						warnings = true;
					}
				}
				
				//Ask whether the user wants to display warnings?
				if(warnings){
					bool newSetting = EditorGUILayout.Toggle("Show warnings", showWarnings);
					if(newSetting != showWarnings){
						//commit to editor prefs
						EditorPrefs.SetBool("HazeWarnings", newSetting);
					}
				}
				
			}
		}
	}
#endif
	
}