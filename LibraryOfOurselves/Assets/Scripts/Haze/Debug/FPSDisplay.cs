using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSDisplay : MonoBehaviour {
	
	Text text;
	
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
				text.text = "FPS: " + frames;
				elapsed -= 1.0f;
				frames = 0;
			}
			yield return null;
		}
	}
	
}
