using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TypeText : MonoBehaviour {
	
	[SerializeField] string text = "[Hello world]";
	[SerializeField] List<float> waitBetweenChars;
	[SerializeField] bool onStart = true;
	[SerializeField] bool onEnable = false;
	[SerializeField] UnityEngine.Events.UnityEvent onEnd;
	
	TextMesh tm = null;
	
	void Start() {
		if(onStart)
			Type();
	}

	private void OnEnable() {
		if(onEnable)
			Type();
	}

	private void init() {
		if(waitBetweenChars == null || waitBetweenChars.Count < 1) {
			Debug.LogError("Wait Between Chars should have at least one element!");
			waitBetweenChars = new List<float>() { 0.05f };
		}
		tm = GetComponent<TextMesh>();
	}

	public void Type(string t = ""){
		if(tm == null) init();
		if(t == "") t = text;
		StartCoroutine(type(t));
	}
	
	IEnumerator type(string t){
		tm.text = "";
		for(int i = 1; i<=t.Length; ++i){
			tm.text = t.Substring(0, i);
			float duration = waitBetweenChars[(int)Mathf.Repeat(i - 1, waitBetweenChars.Count)];
			if(duration > 0) yield return new WaitForSeconds(duration);
		}
		onEnd.Invoke();
	}
	
}
