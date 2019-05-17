/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Haze{
	/** Made to be used as an example alongside LanguageSetter. See default LanguageOption prefab. */
	[ExecuteInEditMode]
	public class LanguageOption : MonoBehaviour {
		
		[SerializeField] Language language;
		[SerializeField] UnityEvent onSelected;
		
		#if UNITY_EDITOR
		[SerializeField] Text text;
		void BakeLanguage(Language lang){
			language = lang;
			text.text = ""+lang.ToString();
		}
		#endif
		
		void OnEnable(){
			if(Languages.Current == language)
				onSelected.Invoke();
		}
		
		public void OnClickButton(){
			//change to language
			Languages.Current = language;
			onSelected.Invoke();
		}
		
	}
}