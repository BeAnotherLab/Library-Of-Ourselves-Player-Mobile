using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IncreaseFillAmount : MonoBehaviour {
	
	[SerializeField] float min = 0;
	[SerializeField] float max = 1;
	[SerializeField] float speed = 1;
	
	Image image = null;
	
	void Set(float val){
		if(image == null) image = GetComponent<Image>();
		image.fillAmount = val;
	}
	
	public void MaxOut(){
		Set(max);
	}
	
	public void MinOut(){
		Set(min);
	}
	
	public void Increase(){
		StopAllCoroutines();
		StartCoroutine(increase());
	}
	
	IEnumerator increase(){
		float v = image.fillAmount;
		
		while(v < max){
			v += Time.deltaTime * speed;
			if(v > max) v = max;
			Set(v);
			yield return null;
		}
	}
	
	public void Decrease(){
		StopAllCoroutines();
		StartCoroutine(decrease());
	}
	
	IEnumerator decrease(){
		float v = image.fillAmount;
		
		while(v > min){
			v -= Time.deltaTime * speed;
			if(v < min) v = min;
			Set(v);
			yield return null;
		}
	}
	
}
