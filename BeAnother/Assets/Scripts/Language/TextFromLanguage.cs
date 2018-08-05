using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class TextFromLanguage : MonoBehaviour {
	
	[SerializeField] string key;
	
	void Start(){
		Text t = GetComponent<Text>();
		Type language = typeof(Lang.Language);
		FieldInfo field = language.GetField(key);
		Lang.Language currentLanguage = Lang.Uage;
		string str = (string)field.GetValue(currentLanguage);
		t.text = str;
	}
	
}
