using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSDisplay : MonoBehaviour {
	
	Text text;
	[SerializeField] float showFPSOnlyUnderneath = 1000;
	
	void Start(){
		text = GetComponent<Text>();
		StartCoroutine(FPS());
	}
	
	IEnumerator FPS(){
		float frames = 0;
		float elapsed = 0;
		while(true){
			elapsed += Time.unscaledDeltaTime;
			++frames;
			if(elapsed >= 1.0f){
				if(frames < showFPSOnlyUnderneath) {
					text.text = "FPS: " + frames;
				} else
					text.text = "";
				elapsed -= 1.0f;
				frames = 0;
			}
			yield return null;
		}
	}
	
}
