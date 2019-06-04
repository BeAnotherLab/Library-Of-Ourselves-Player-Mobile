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
	public class TranslatedElement : MonoBehaviour {
		
		[SerializeField] public List<Translation> translations = new List<Translation>();
		
		[Serializable]
		public class Translation{
			public Language language;
			public Font font;
			public string contents;
		}
		
		bool initialised = false;
		bool translated = false;
		Text uiText = null;
		TextMesh textMesh = null;
		Font defaultFont = null;
		
		/** You can use the value from this field to get the translated contents of whatever element you're dealing with. */
		public string Contents{
			private set{
				Init();
				if(uiText) uiText.text = value;
				else if(textMesh) textMesh.text = value;
				else name = value;
			}
			get{
				Init();
				Translate();
				if(uiText) return uiText.text;
				else if(textMesh) return textMesh.text;
				else return name;
			}
		}
		
		void Awake(){
			Init();
			Translate();
		}
		
		void Init(){
			if(initialised) return;
			initialised = true;
			
			uiText = GetComponent<Text>();
			if(uiText != null){
				defaultFont = uiText.font;
			}else{
				textMesh = GetComponent<TextMesh>();
				if(textMesh != null){
					defaultFont = textMesh.font;
				}
			}
		}
		
		void Translate(){
			if(translated) return;
			translated = true;
			
			//set Contents according to current language
			foreach(Translation t in translations){
				if(t.language == Languages.Current){
					if(t.contents == "") break;//doesn't allow an empty string as the contents.
					Contents = t.contents;
					//set font on element:
					if(uiText != null){
						if(t.font != null){
							uiText.font = t.font;
						}else{
							uiText.font = defaultFont;
						}
					}else if(textMesh != null){
						if(t.font != null){
							textMesh.font = t.font;
						}else{
							textMesh.font = defaultFont;
						}
					}
					return;
				}
			}
			//none found!
			Contents = "???[NO TRANSLATION FOR " + Languages.Current + "]???";
			
			
			/*if(translations.ContainsKey(Languages.Current) && translations[Languages.Current] != "")
				Contents = translations[Languages.Current];
			else
				Contents = "???[NO TRANSLATION FOR " + Languages.Current + "]???";
			*///The above is made inefficient because Unity doesn't serialize Dictionaries. Extracting the Dictionary at runtime would cost more than going through the List.
			//(Assuming language isn't changed too many times in one scene, that is.)
			
		}
		
		/** Called by Languages on every TranslatedElement whenever the user (or program) changes the current language. */
		public void OnLanguageChange(){
			translated = false;
			Translate();
		}

		private void OnEnable() {
			OnLanguageChange();
		}

	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(TranslatedElement))]
	public class TranslatedElementInspector : Editor {
		
		AutoTranslation autoTranslation = null;//currently auto-translating something when this isnt null
		
		string defaultContents(TranslatedElement elem){
			if(elem.GetComponent<Text>()){
				return elem.GetComponent<Text>().text;
			}else if(elem.GetComponent<TextMesh>()){
				return elem.GetComponent<TextMesh>().text;
			}else{
				return elem.name;
			}
		}
		
		public override void OnInspectorGUI(){
			serializedObject.Update();
			TranslatedElement elem = target as TranslatedElement;
			Undo.RecordObject(elem, "Modified translation");
			
			if(SupportedLanguages.Instance != null){
				
				//display what this TranslatedElement will translate:
				string willTranslate = "Will translate ";
				if(elem.GetComponent<Text>()){
					willTranslate += "UI Text component on";
				}else if(elem.GetComponent<TextMesh>()){
					willTranslate += "Text Mesh component on";
				}else{
					willTranslate += "name of";
				}
				willTranslate += "\nthis GameObject.";
				GUILayout.Label(willTranslate);
				GUILayout.Space(10);
				
				foreach(SupportedLanguages.SupportedLanguage language in SupportedLanguages.Instance.supportedLanguages){
					GUILayout.BeginHorizontal();
					GUILayout.Label(""+language.language);
					if(language.font != null){
						GUIStyle style = new GUIStyle();
						style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
						GUILayout.Label("(uses font " + language.font.name+")", style);
					}
					GUILayout.EndHorizontal();
					bool found = false;
					foreach(TranslatedElement.Translation t in elem.translations){
						if(t.language == language.language){
							found = true;
							t.font = language.font;
							if(language.language == SupportedLanguages.Instance.defaultLanguage){
								if(!EditorApplication.isPlaying)
									t.contents = defaultContents(elem);
								GUILayout.Label(t.contents);
							}else{
								t.contents = EditorGUILayout.TextField(t.contents);
								if(language.translate && t.contents == ""){
									if(autoTranslation == null && defaultContents(elem) != ""){
										//let's start auto-translating this!
										//Attempt to find an active monobehaviour in the scene to handle the coroutine.
										Transform coroutineElem = elem.transform;
										while(!coroutineElem.gameObject.activeInHierarchy) {
											if(coroutineElem.parent == null)
												break;
											coroutineElem = coroutineElem.parent;
										}
										while(coroutineElem.GetComponent<MonoBehaviour>() == null) {
											//find the first active child with a monobehaviour
											if(coroutineElem.childCount <= 0)
												break;//no luck :'(
											foreach(Transform child in coroutineElem) {
												if(child.gameObject.activeInHierarchy) {
													if(child.GetComponent<MonoBehaviour>() != null) {
														coroutineElem = child;
														break;
													}
												}
											}
										}
										autoTranslation = new AutoTranslation(defaultContents(elem), SupportedLanguages.Instance.defaultLanguage, t.language, coroutineElem.GetComponent<MonoBehaviour>());
									}else if(autoTranslation.Finished && autoTranslation.TargetLanguage == t.language){
										//done translating this language!
										t.contents = autoTranslation.Translated;
										autoTranslation = null;
									}
								}
							}
							break;
						}
					}
					if(!found){
						TranslatedElement.Translation t = new TranslatedElement.Translation();
						t.language = language.language;
						if(language.language == SupportedLanguages.Instance.defaultLanguage){
							if(!EditorApplication.isPlaying)
								t.contents = defaultContents(elem);
							GUILayout.Label(t.contents);
						}else
							t.contents = EditorGUILayout.TextField("");
						elem.translations.Add(t);
					}
				}
				
				//check whether any translations are provided for unsupported languages, and delete them.
				foreach(TranslatedElement.Translation t in elem.translations){
					bool found = false;
					foreach(SupportedLanguages.SupportedLanguage lang in SupportedLanguages.Instance.supportedLanguages){
						if(lang.language == t.language){
							found = true;
							break;
						}
					}
					if(!found){
						elem.translations.Remove(t);
						Debug.LogWarning("Removed translation " + t.language + " from " + elem.name);
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
		
		void OnInspectorUpdate(){
			Repaint();
		}
		
	}
	#endif
}