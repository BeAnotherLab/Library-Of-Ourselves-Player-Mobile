using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Haze{
	public static class EditableText {
		
		///Attempts to set a UI text or TextMesh on the specified object with the specified string.
		public static void SetText(this GameObject go, string text){
            text = text.Replace("[\\n]", "\n");
			Text tUi = go.GetComponentInChildren<Text>();
			if(tUi){
				tUi.text = text;
				return;
			}
			TextMesh tMesh = go.GetComponentInChildren<TextMesh>();
			if(tMesh) tMesh.text = text;
			else go.name = text;//if none is available, just use GameObject's name.
		}
		
	}
}