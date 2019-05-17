/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

#pragma warning disable 0618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Haze{
	/** Place this MonoBehaviour on a GameObject in your Settings scene
		Change display languages by calling SetLanguage (from a 
		UnityEvent fired by a UI Button, for example).					*/
	public class LanguageSetter : MonoBehaviour {
		
		#if UNITY_EDITOR
		public GameObject prefab = null;
		
		/** Called when pressing Bake in inspector */
		public void Bake(){
			if(transform.childCount != 0){
				if(!EditorUtility.DisplayDialog(name + " is not empty", "Are you sure you want to bake this LanguageSetter? Its children will be deleted.", "ok", "cancel")){
					return;
				}
			}
			if(SupportedLanguages.Instance == null){
				if(EditorUtility.DisplayDialog("Missing languages", "You do not have an asset of type SupportedLanguages in your project. Create one?", "ok", "cancel")){
					SupportedLanguages.ShowSupportedLanguagesMenu();
				}
				return;
			}
			while(transform.childCount > 0)
				DestroyImmediate(transform.GetChild(0).gameObject);
			//duplicated prefab for each supported language
			foreach(SupportedLanguages.SupportedLanguage lang in SupportedLanguages.Instance.supportedLanguages){
				GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				go.transform.SetParent(transform, false);
				go.name = prefab.name+" ("+lang.language+")";
				go.SendMessage("BakeLanguage", lang.language, SendMessageOptions.DontRequireReceiver);
				EditorUtility.SetDirty(go);
			}
			EditorUtility.SetDirty(this);
			EditorSceneManager.MarkAllScenesDirty();
			
			if(EditorUtility.DisplayDialog("Language options provided", "This Language Setter has done its job. It is safe to remove it now. Do you want us to remove it?", "Yes", "I'll keep it for now")){
				DestroyImmediate(this);
			}
		}
		#else
		void Awake(){
			Destroy(this);
		}
		#endif
		
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LanguageSetter))]
	public class LanguageSetterInspector : Editor {
		
		public override void OnInspectorGUI(){
			LanguageSetter t = (LanguageSetter)target;
			GUIStyle style = new GUIStyle();
			style.wordWrap = true;
			if(EditorApplication.isPlaying){
				GUILayout.Label("Current language: " + Languages.Current);
				if(SupportedLanguages.Instance != null){
					foreach(SupportedLanguages.SupportedLanguage lang in SupportedLanguages.Instance.supportedLanguages){
						if(GUILayout.Button("Switch to " + lang.language))
							Languages.Current = lang.language;
					}
				}else{
					GUILayout.Label("You do not have an asset of type SupportedLanguages yet.");
					if(GUILayout.Button("Create one")){
						SupportedLanguages.ShowSupportedLanguagesMenu();
					}
				}
			}else{
				GUILayout.Label("You can emulate changing languages from this inspector in play mode.", style);
				GUILayout.Space(15);
				GUILayout.Label("Add your prefab to this component, then press Bake, to instantiate your prefab under this Transform once for each supported language your game has. Once instantiated, the GameObject will immediately receive a BakeLanguage(Language) message.", style);
				GUILayout.Space(5);
				var prefabProperty = serializedObject.FindProperty("prefab");
				EditorGUILayout.PropertyField(prefabProperty, GUIContent.none);
				serializedObject.ApplyModifiedProperties();
				if(t.prefab != null){
					if(PrefabUtility.GetPrefabType(t.prefab) == PrefabType.Prefab){
						if(GUILayout.Button("Bake")){
							t.Bake();
						}
					}else{
						GUILayout.Label("Not a prefab.");
					}
				}
				EditorUtility.SetDirty(t);
			}
		}
		
	}
	#endif
}