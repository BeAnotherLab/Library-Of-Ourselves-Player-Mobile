using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TypeText : MonoBehaviour {
	
	[SerializeField] string text = "[Hello world]";
	[SerializeField] List<float> waitBetweenChars;
	[SerializeField] bool onStart = true;
	
	TextMesh tm;
	
	void Start(){
		if(waitBetweenChars == null || waitBetweenChars.Count < 1){
			Debug.LogError("Wait Between Chars should have at least one element!");
			waitBetweenChars = new List<float>(){ 0.05f };
		}
		tm = GetComponent<TextMesh>();
		if(onStart)
			Type();
	}
	
	public void Type(string t = ""){
		if(t == "") t = text;
		StartCoroutine(type(t));
	}
	
	IEnumerator type(string t){
		tm.text = "";
		for(int i = 1; i<=t.Length; ++i){
			tm.text = t.Substring(0, i);
			yield return new WaitForSeconds(waitBetweenChars[(int)Mathf.Repeat(i-1, waitBetweenChars.Count)]);
		}
	}
	
}
