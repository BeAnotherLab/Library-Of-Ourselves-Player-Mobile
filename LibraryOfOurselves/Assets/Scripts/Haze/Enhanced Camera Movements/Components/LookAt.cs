/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Enhanced Camera Movements".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

///Place this component on your camera to keep its view centered onto a particular object.
namespace Haze{
	[RequireComponent(typeof(Camera))]
	public class LookAt : MonoBehaviour {
		
		[SerializeField] Transform target;//what we will look at
		[SerializeField] float minimumDistance = 0;
		[SerializeField] bool useColliderDistance = false;//turn this off for performance
		[SerializeField] bool modifyDepthOfField = false;
		
		///
		/// Properties
		///
		
		public Transform Target {
			get{ return target; }
			set{ target = value; }
		}
		
		public float MinimumDistance {
			get{ return minimumDistance; }
			set{ minimumDistance = value; }
		}
		
		public bool UseColliderDistance {
			get{ return useColliderDistance; }
			set{ useColliderDistance = value; }
		}
		
		public bool ModifyDepthOfField {
			get{ return modifyDepthOfField; }
			set{ modifyDepthOfField = value; }
		}
		
		///
		/// Functions
		///
		
		static Vector3 getTargetPosition(Transform camT, Transform target, Collider targetColl = null){
			Vector3 targetPosition;//world space
			if(targetColl){//use the closest point on the collider if available (warning: less performance)
				targetPosition = targetColl.ClosestPointOnBounds(camT.position);
			}else{
				targetPosition = target.position;
			}
			return targetPosition;
		}
		
		/// Look at our target as long as the LookAt component is enabled
		void Update(){
			Look(GetComponent<Camera>(), target, minimumDistance, useColliderDistance, modifyDepthOfField);
		}
		
		/// Start looking continuously at the target
		/// Use from a MonoBehaviour as StartCoroutine(Haze.LookAt.Look(...));
		public static IEnumerator look(Camera cam, Transform target, float minimumDistance = 0, bool useColliderDistance = false, bool modifyDepthOfField = false){
			while(true)
				Look(cam, target, minimumDistance, useColliderDistance, modifyDepthOfField);
		}
		
		/// Look at the target specified for the frame. Call in update to continuously look at it.
		public static void Look(Camera cam, Transform target, float minimumDistance = 0, bool useColliderDistance = false, bool modifyDepthOfField = false){
			if(cam == null) cam = Camera.main;
			
			// checks
			if(!target){
				Debug.LogWarning("Cannot execute look at; target is null.");
				return;
			}
			
			// cache
			Transform camT = cam.transform;
			Collider targetColl = null;
			if(useColliderDistance) targetColl = target.GetComponent<Collider>();
			
			camT.rotation = Quaternion.LookRotation(target.position - camT.position);
			//fix distance
			if(minimumDistance > 0){//ensure to keep far enough from target if needed
				Vector3 targetPos = getTargetPosition(camT, target, targetColl);
				Vector3 camToTarget = targetPos - camT.position;
				float distance = camToTarget.sqrMagnitude;
				if(distance < minimumDistance * minimumDistance){
					camT.position = targetPos - camToTarget.normalized * minimumDistance;
				}
			}
			
			//focus if needed
			if(modifyDepthOfField)
				Focus.Autofocus(cam, target, useColliderDistance);
			
		}
		
	}
	
#if UNITY_EDITOR
	[CustomEditor(typeof(LookAt))]
	public class LookAtEditor : Editor {
		public override void OnInspectorGUI(){
			
			DrawDefaultInspector();
			
			LookAt lookAt = target as LookAt;
			if(lookAt){
				
				//Show warnings if needed
				bool showWarnings = !EditorPrefs.HasKey("HazeWarnings") || EditorPrefs.GetBool("HazeWarnings") == true;
				bool warnings = false;//true if there's at least one warning.
				GUIStyle warningStyle = new GUIStyle();
				warningStyle.wordWrap = true;
				warningStyle.normal.textColor = new Color(1, 0.3f, 0);
				if(lookAt.ModifyDepthOfField){
					#if !HAZE_POSTPROCESSING
					if(showWarnings) EditorGUILayout.LabelField("Warning: depth of field won't be modified. If you use the Unity Post Processing Stack, you need to add HAZE_POSTPROCESSING to the Scripting Define Symbols in your project's Player Settings.", warningStyle);
					warnings = true;
					#endif
					if(!lookAt.UseColliderDistance){
						if(showWarnings) EditorGUILayout.LabelField("Warning: depth of field will work better if you enable Use Collider Distance (as long as the target has a Collider component attached).", warningStyle);
						warnings = true;
					}
					if(lookAt.GetComponent<Focus>() != null){
						if(showWarnings) EditorGUILayout.LabelField("Warning: Modify Depth Of Field is enabled but this camera already has a Focus component. Get rid of the Focus component, or disable Modify Depth Of Field.", warningStyle);
						warnings = true;
					}
				}else if(lookAt.UseColliderDistance){
					if(showWarnings) EditorGUILayout.LabelField("Warning: distance will be taken from collider, which will affect performance; you didn't enable Modify Depth Of Field, so you may want to disable Use Collider Distance as well.", warningStyle);
					warnings = true;
				}
				if(lookAt.Target == null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: Target is null.", warningStyle);
					warnings = true;
				}
				else if(lookAt.UseColliderDistance && lookAt.Target.GetComponent<Collider>() == null){
					if(showWarnings) EditorGUILayout.LabelField("Warning: you enabled Use Collider Distance, however the target does not have a collider component attached.", warningStyle);
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
