/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Haze{
	public class LocalizedSprite : MonoBehaviour {
		
		[SerializeField] public List<Translation> translations = new List<Translation>();
		
		[Serializable]
		public class Translation{
			public Language language;
			public Sprite contents;
		}
		
		bool initialised = false;
		bool translated = false;
		Image uiImage = null;
		SpriteRenderer spriteRenderer = null;
		
		public Sprite Contents{
			get{
				if(uiImage != null) return uiImage.sprite;
				else if(spriteRenderer != null) return spriteRenderer.sprite;
				else return null;
			}
			private set{
				if(uiImage != null){
					uiImage.sprite = value;
				}else if(spriteRenderer != null){
					spriteRenderer.sprite = value;
				}else{
					Debug.LogError("LocalizedSprite " + name + " needs a UI Image or SpriteRenderer component to work.");
				}
			}
		}
		
		void Awake(){
			Init();
			Translate();
		}
		
		void Init(){
			if(initialised) return;
			initialised = true;
			
			uiImage = GetComponent<Image>();
			if(uiImage == null){
				spriteRenderer = GetComponent<SpriteRenderer>();
				if(spriteRenderer == null){
				}
			}
		}
		
		void Translate(){
			if(translated) return;
			translated = true;
			
			//fetch translated contents
			foreach(Translation t in translations){
				if(t.language == Languages.Current){
					Contents = t.contents;
					return;
				}
			}
			//none found...
			Debug.LogError("Couldn't get the localized sprite for " + Languages.Current);
		}
		
		/** Called by Languages on every LocalizedSprite whenever the user (or program) changes the current language. */
		public void OnLanguageChange(){
			translated = false;
			Translate();
		}
		
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(LocalizedSprite))]
	public class LocalizedSpriteInspector : Editor {
		
		Sprite defaultContents(LocalizedSprite elem){
			if(elem.GetComponent<Image>()){
				return elem.GetComponent<Image>().sprite;
			}else if(elem.GetComponent<SpriteRenderer>()){
				return elem.GetComponent<SpriteRenderer>().sprite;
			}else{
				return null;
			}
		}
		
		public override void OnInspectorGUI(){
			serializedObject.Update();
			LocalizedSprite elem = target as LocalizedSprite;
			Undo.RecordObject(elem, "Modified localized sprite");
			
			if(SupportedLanguages.Instance != null){
				
				//display what this LocalizedSprite will translate:
				string willTranslate = "Will localize ";
				bool ok = true;
				if(elem.GetComponent<Image>()){
					willTranslate += "UI Image component on";
				}else if(elem.GetComponent<SpriteRenderer>()){
					willTranslate += "SpriteRenderer component on";
				}else{
					ok = false;//can't do that ya'll!
				}
				willTranslate += "\nthis GameObject.";
				if(!ok){
					GUIStyle style = new GUIStyle();
					style.wordWrap = true;
					GUILayout.Label("LocalizedSprites need to be on a GameObject with a UI Image or SpriteRenderer component attached!", style);
				}
				GUILayout.Label(willTranslate);
				GUILayout.Space(10);
				
				foreach(SupportedLanguages.SupportedLanguage language in SupportedLanguages.Instance.supportedLanguages){
					GUILayout.Label(""+language.language);
					bool found = false;
					foreach(LocalizedSprite.Translation t in elem.translations){
						if(t.language == language.language){
							found = true;
							if(language.language == SupportedLanguages.Instance.defaultLanguage){
								if(!EditorApplication.isPlaying)
									t.contents = defaultContents(elem);
								if(t.contents == null)
									t.contents = (Sprite)EditorGUILayout.ObjectField(t.contents, typeof(Sprite), true);
								else
									GUILayout.Label("(uses default sprite " + t.contents.name + ")");
							}else{
								t.contents = (Sprite)EditorGUILayout.ObjectField(t.contents, typeof(Sprite), true);
							}
							break;
						}
					}
					if(!found){
						LocalizedSprite.Translation t = new LocalizedSprite.Translation();
						t.language = language.language;
						if(language.language == SupportedLanguages.Instance.defaultLanguage){
							if(!EditorApplication.isPlaying)
								t.contents = defaultContents(elem);
							if(t.contents == null)
								t.contents = (Sprite)EditorGUILayout.ObjectField(null, typeof(Sprite), true);
							else
								GUILayout.Label("(uses default sprite " + t.contents.name + ")");
						}else
							t.contents = (Sprite)EditorGUILayout.ObjectField((Sprite)null, typeof(Sprite), true);
						elem.translations.Add(t);
					}
				}
				
				//check whether any translations are provided for unsupported languages, and delete them.
				foreach(LocalizedSprite.Translation t in elem.translations){
					bool found = false;
					foreach(SupportedLanguages.SupportedLanguage lang in SupportedLanguages.Instance.supportedLanguages){
						if(lang.language == t.language){
							found = true;
							break;
						}
					}
					if(!found){
						elem.translations.Remove(t);
						Debug.LogWarning("Removed localized sprite " + t.language + " from " + elem.name);
						break;
					}
				}
				
				if(GUILayout.Button("Edit supported languages")){
					SupportedLanguages.ShowSupportedLanguagesMenu();
				}
			}else{
				if(GUILayout.Button("Create supported languages")){
					SupportedLanguages.ShowSupportedLanguagesMenu();
				}
			}
			
			serializedObject.ApplyModifiedProperties();
		}
		
	}
	#endif
}
