using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideByPixelSize : MonoBehaviour {
	
	[SerializeField] float input = 1;
	[SerializeField] Division division = Division.DivideByRectTransformWidth;
	[SerializeField] RectTransform rectTransform;
	[SerializeField] FloatEvent output;
	[SerializeField] bool inUpdate = true;
	[SerializeField] bool inUpdateMobile = false;
	
	enum Division{
		DivideByRectTransformWidth,
		DivideByRectTransformHeight
	}
	
	public void Divide(){
		float result = input;
		switch(division){
			case Division.DivideByRectTransformWidth:
				result /= rectTransform.rect.width;
				break;
			case Division.DivideByRectTransformHeight:
				result /= rectTransform.rect.height;
				break;
		}
		output.Invoke(result);
	}
	
	void Start(){
		if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
		if(rectTransform == null) Debug.LogError("You must set Rect Transform to something.");
		
		#if UNITY_EDITOR
		if(inUpdateMobile){
			Debug.LogWarning("Updating constantly by pixel size might not be wanted on mobile platforms.");
		}
		#endif
		
		Divide();
	}
	
	void Update(){
		#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		if(inUpdateMobile) Divide();
		#else
		if(inUpdate) Divide();
		#endif
	}
	
}
