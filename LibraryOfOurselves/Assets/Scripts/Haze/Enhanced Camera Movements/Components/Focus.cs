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

///Place this component on your camera to add the ability to focus onto specific GameObjects. Needs HAZE_POSTPROCESSING defined and the Unity Post Processing Stack in use in the project.
namespace Haze{
	[RequireComponent(typeof(Camera))]
	public class Focus : MonoBehaviour {
		
		[SerializeField] Transform target;//what we will focus on
		[SerializeField] bool useColliderDistance = true;//turn this off for performance
		
		///
		/// Properties
		///
		
		public Transform Target {
			get{ return target; }
			set{ target = value; }
		}
		
		public bool UseColliderDistance {
			get{ return useColliderDistance; }
			set{ useColliderDistance = value; }
		}
		
		///
		/// Functions
		///
		
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
		
		/// Autofocuses on update
		void Update(){
			Autofocus(GetComponent<Camera>(), target, useColliderDistance);
		}
		
		/// Autofocus on target
		public static void Autofocus(Camera cam, Transform target, bool useColliderDistance = true){
			#if HAZE_POSTPROCESSING
			if(cam == null) cam = Camera.main;
			
			if(target == null){
				Debug.LogWarning("Cannot focus on anything; target is null.");
				return;
			}
			
			Collider targetColl = null;
			if(useColliderDistance) targetColl = target.GetComponent<Collider>();
			
			float currentDistance = distanceToTarget(cam.transform, target, targetColl);
			//set depth of field focus distance if we have post processing stack
			PostProcessingBridge.SetDepthOfField_FocusDistance(currentDistance, cam);
			#endif
		}
		public static void Autofocus(Transform target, bool useColliderDistance = true){
			Autofocus(null, target, useColliderDistance);
		}
		
	}
	
#if UNITY_EDITOR
	[CustomEditor(typeof(Focus))]
	public class FocusEditor : Editor {
		public override void OnInspectorGUI(){
			
			DrawDefaultInspector();
			
			Focus focus = target as Focus;
			if(focus){
				
				//Show warnings if needed
				bool showWarnings = !EditorPrefs.HasKey("HazeWarnings") || EditorPrefs.GetBool("HazeWarnings") == true;
				bool warnings = false;//true if there's at least one warning.
				GUIStyle warningStyle = new GUIStyle();
				warningStyle.wordWrap = true;
				warningStyle.normal.textColor = new Color(1, 0.3f, 0);
				#if !HAZE_POSTPROCESSING
				if(showWarnings) EditorGUILayout.LabelField("Warning: depth of field won't be modified. If you use the Unity Post Processing Stack, you need to add HAZE_POSTPROCESSING to the Scripting Define Symbols in your project's Player Settings.", warningStyle);
				warnings = true;
				#endif
				if(focus.Target == null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: Target is null.", warningStyle);
					warnings = true;
				}
				else if(focus.UseColliderDistance && focus.Target.GetComponent<Collider>() == null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: you enabled Use Collider Distance, however the target does not have a collider component attached.", warningStyle);
					warnings = true;
				}
				else if(!focus.UseColliderDistance && focus.Target.GetComponent<Collider>() != null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: depth of field will work better if you enable Use Collider Distance (as long as the target has a Collider component attached).", warningStyle);
					warnings = true;
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