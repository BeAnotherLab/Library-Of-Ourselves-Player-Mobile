/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Haze{
	
	/** Create a new asset in your project of type SupportedLanguages; this is required for most of the functionality of Haze Translations */
	#if UNITY_EDITOR
	[CreateAssetMenu(menuName="Haze/Supported Languages")]
	[InitializeOnLoad]
	#endif
	public class SupportedLanguages : ScriptableObject {
		
		static SupportedLanguages instance = null;
		public static SupportedLanguages Instance{
			get{
				if(instance == null){
					#if UNITY_EDITOR
					instance = GetFirstInstance();//attempt to find ones
					#else
					Debug.LogError("No instance of SupportedLanguages! Please refer to the Haze Easy Translations and Localization documentation to fix this.");
					#endif
				}
				return instance;
			}
		}
		
		#if UNITY_EDITOR
		/** find first instance of a SupportedLanguages asset in the project. */
		static SupportedLanguages GetFirstInstance(){
			string[] guids = AssetDatabase.FindAssets("t:SupportedLanguages");
			if(guids.Length > 0){
				string path = AssetDatabase.GUIDToAssetPath(guids[0]);
				return AssetDatabase.LoadAssetAtPath<SupportedLanguages>(path);
			}else{
				return null;
			}
		}
		#endif
		
		[Tooltip("The default language you will be developing in.")] public Language defaultLanguage = Language.English;
		public List<SupportedLanguage> supportedLanguages = new List<SupportedLanguage>();
		
		[Serializable]
		public class SupportedLanguage{
			public Language language;
			public Font font;
			public bool translate;
			
			public SupportedLanguage(Language lang = Language.English){
				language = lang;
				font = null;
				translate = false;
			}
		}
		
		void OnEnable(){
			if(instance == this) return;//already Enabled.
			if(instance != null){
				Debug.LogError("You should only have one asset of type SupportedLanguages in your project!");
				return;
			}
			instance = this;
		}
		
		//checks whether this object is in the preloaded assets
		#if UNITY_EDITOR && UNITY_2018_2_OR_NEWER
		public void MaintainIntegrity(){
			//use the newer api
			var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
			if(!preloadedAssets.Contains(this)){
				Debug.Log("Adding " + name + " to preloaded assets. This is required for Haze Translations and Localization to work in your builds. If you're not using Haze Translations and Localization in your project, please consider deleting " + name + ".asset from the Assets.");
				//gotta add it!
				preloadedAssets.Add(this);
				//and remove any null object too
				preloadedAssets.RemoveAll(item => item == null);
				PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
			}
		}
		#elif UNITY_EDITOR
		private double errorLogTime = 0;//preventing older versions of unity from logging the same message again and again and again :)
		public void MaintainIntegrity(){
			//versions prior to Unity 2018.2 do not expose GetPreloadedAssets() and SetPreloadedAssets() in UnityEditor.PlayerSettings.
			SerializedObject playerSettings = new SerializedObject(Resources.FindObjectsOfTypeAll<PlayerSettings>());
			var preloadedAssets = playerSettings.FindProperty("preloadedAssets");
			if(preloadedAssets != null && preloadedAssets.isArray){
				bool found = false;
				bool modified = false;
				List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
				for(int i = 0; i<preloadedAssets.arraySize; ++i){
					UnityEngine.Object elem = preloadedAssets.GetArrayElementAtIndex(i).objectReferenceValue;
					if(elem == this){
						found = true;
					}
					if(elem != null)
						assets.Add(elem);
					else modified = true;
				}
				if(!found){
					if(EditorApplication.timeSinceStartup - errorLogTime > 1.0f){//only log this if we haven't done so in the last second
						Debug.Log("Adding " + name + " to preloaded assets. This is required for Haze Translations and Localization to work in your builds. If you're not using Haze Translations and Localization in your project, please consider deleting " + name + ".asset from the Assets.");
					}
					errorLogTime = EditorApplication.timeSinceStartup;
					//gotta add it!
					assets.Add(this);
					modified = true;
				}
				if(modified){
					//we'll have to write this back out now that we've modified it.
					preloadedAssets.arraySize = assets.Count;
					for(int i = 0; i<assets.Count; ++i){
						var e = preloadedAssets.GetArrayElementAtIndex(i);
						e.objectReferenceValue = assets[i];
					}
					playerSettings.ApplyModifiedProperties();
					AssetDatabase.SaveAssets();
				}
				
			}else{
				Debug.LogError("Could not find preloaded assets in player settings.");
			}
		}
		#endif
		
		void OnDestroy(){
			if(instance == this) instance = null;
		}
		
		public static bool IsLanguageSupported(Language lang){
			if(Instance != null){
				foreach(SupportedLanguage sl in Instance.supportedLanguages){
					if(sl.language == lang) return true;
				}
			}else Debug.LogError("No instance of SupportedLanguages! Please refer to the Haze Easy Translations and Localization documentation to fix this.");
			return false;
		}
		
		public static Language DefaultLanguage{ get{ if(Instance != null) return Instance.defaultLanguage; Debug.LogError("No instance of SupportedLanguages! Please refer to the Haze Easy Translations and Localization documentation to fix this."); return Language.English; } }
		
		#if UNITY_EDITOR
		/** Opens the inspector for the current Languages asset, or create a new one if there's none in this project. */
		public static void ShowSupportedLanguagesMenu(){
			if(Instance != null){
				Selection.activeObject = Instance;//show inspector
			}else{
				//Create the new asset:
				SupportedLanguages languages = ScriptableObject.CreateInstance<SupportedLanguages>();
				languages.supportedLanguages = new List<SupportedLanguage>(){ new SupportedLanguage(Language.English) };
				AssetDatabase.CreateAsset(languages, "Assets/Languages.asset");
				Debug.Log("Created Assets/Languages.asset");
				Selection.activeObject = languages;//show inspector
			}
		}
		#endif
		
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(SupportedLanguages))]
	public class SupportedLanguagesInspector : Editor{
		
		//the language staged for adding
		Language selected = Language.English;
		//the sample we have created (null if user hasn't requested one yet)
		LanguageSamples.Sample sample = null;
		Font sampleFont = null;
		int sampleFontSize = 30;
		
		[InitializeOnLoadMethod]
		public static void InitUpdate() { EditorApplication.update += Update; }
		
		static void Update(){
			if(SupportedLanguages.Instance != null)
				SupportedLanguages.Instance.MaintainIntegrity();
		}
		
		public override void OnInspectorGUI(){
			SupportedLanguages t = target as SupportedLanguages;
			if(t == null) return;
			if(t != SupportedLanguages.Instance){
				GUILayout.Label("You should only have one asset of\ntype SupportedLanguages in your project!");
				GUILayout.Label("Delete this asset.");
				return;
			}
			
			bool possibleTranslation = true;//will be set to false if default language is not supported by AutoTranslation
			
			//Setting default language:
			GUILayout.Label("Select your own default language.");
			GUILayout.Label("This is the language you make your\ninterfaces in Unity in.");
			GUILayout.Label("For example if you have UI Texts with\ntheir text property in French, select French.");
			t.defaultLanguage = (Language)EditorGUILayout.EnumPopup(t.defaultLanguage);
			if(AutoTranslation.TranslationLanguageCode(t.defaultLanguage) == ""){
				//impossible to translate from this one!
				GUILayout.Label(t.defaultLanguage + " will not support automatic translations.");
				possibleTranslation = false;
			}
			if(t.defaultLanguage == Language.Default) t.defaultLanguage = Language.English;
			//check that default language is among supported languages:
			bool contains = false;
			foreach(SupportedLanguages.SupportedLanguage sl in t.supportedLanguages){
				if(sl.language == t.defaultLanguage){
					contains = true;
					break;
				}
			}
			if(!contains){
				t.supportedLanguages.Add(new SupportedLanguages.SupportedLanguage(t.defaultLanguage));
			}
			
			GUILayout.Space(30);
			
			//Displaying available languages:
			GUILayout.Label("Languages your game will be available in:");
			foreach(SupportedLanguages.SupportedLanguage lang in t.supportedLanguages){
				GUILayout.BeginHorizontal();
				GUILayout.Label("- "+lang.language);
				bool defaultLang = lang.language == t.defaultLanguage;
				if(defaultLang){
					GUIStyle style = new GUIStyle();
					style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
					GUILayout.Label("(Can't remove default language)", style);
				}else{
					if(GUILayout.Button("Remove " + lang.language)){
						t.supportedLanguages.Remove(lang);
						break;
					}
				}
				GUILayout.EndHorizontal();
				
				//Select font:
				GUILayout.BeginHorizontal();
				GUILayout.Label("\tFont for " + lang.language);
				lang.font = (Font)EditorGUILayout.ObjectField(lang.font, typeof(Font), true);
				GUILayout.EndHorizontal();
				
				//Select automatic translation:
				if(!defaultLang && possibleTranslation && AutoTranslation.TranslationLanguageCode(lang.language) != ""){
					GUILayout.BeginHorizontal();
					GUILayout.Label("\tAuto-translate to "+lang.language);
					lang.translate = GUILayout.Toggle(lang.translate, "");
					GUILayout.EndHorizontal();
				}else{
					lang.translate = false;//can't have translations from default to default (or if default can't be translated)
				}
				
				GUILayout.Space(5);
			}
			
			GUILayout.Space(15);
			
			//Adding a language:
			GUILayout.Label("Add a language:");
			GUILayout.BeginHorizontal();
			selected = (Language)EditorGUILayout.EnumPopup(selected);
			if(selected == Language.Default) selected = Language.English;
			//check whether it contains it
			contains = false;
			foreach(SupportedLanguages.SupportedLanguage sl in t.supportedLanguages){
				if(sl.language == selected){
					contains = true;
					break;
				}
			}
			if(contains){
				GUILayout.Label(selected + " is already supported.");
			}else{
				if(GUILayout.Button("Add " + selected)){
					t.supportedLanguages.Add(new SupportedLanguages.SupportedLanguage(selected));
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(20);
			GUILayout.Label("Note: when you modify this asset, it's a\ngood idea to go through all translated\nelements in your project to make sure\neverything looks and behaves fine.");
			GUILayout.Space(20);
			
			//Sample:
			if(GUILayout.Button(sample != null ? "Update sample text" : "Show sample text")){
				sample = new LanguageSamples.Sample();
				foreach(SupportedLanguages.SupportedLanguage lang in t.supportedLanguages){
					sample.AddLanguage(lang.language, lang.font);
				}
				sample.Prepare();
			}
			if(sample != null){
				sampleFont = (Font)EditorGUILayout.ObjectField("Font used for sample", sampleFont, typeof(Font), true);
				Font display = sampleFont;
				if(display == null) display = (Font)Resources.GetBuiltinResource<Font>("Arial.ttf");
				sampleFontSize = (int)EditorGUILayout.Slider("Font size", sampleFontSize, 10, 100);
				GUILayout.Label("Note: this sample might not contain all\ncharacters used in your game and should\nsimply be used as a first idea.");
				sample.DisplayResults(display, sampleFontSize);
			}
			
			EditorUtility.SetDirty(t);
		}
		
	}
	#endif
}