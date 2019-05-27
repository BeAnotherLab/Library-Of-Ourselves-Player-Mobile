using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Haze{

	public class SnapSlider : MonoBehaviour {
		
		[SerializeField] bool snapToInt = true;
		[SerializeField] float snapSpeed = 5;
		[SerializeField] UnityEngine.UI.Slider uiSlider = null;
		[SerializeField] IntEvent onIntValueChange;
		[SerializeField] UnityEvent onSnapped;
		
		float sliderValue = 0;
		int intValue = 0;
		bool needToRound = false;
		
		public float Value{
			get{
				if(snapToInt) return round(sliderValue);
				return sliderValue;
			}
			set{
				uiSlider.value = value;
				OnValueChanged(value);
			}
		}
		
		public int IntValue{
			get{
				return intValue;
			}
		}
		
		int round(float f){
			int intPart = (int)f;
			f -= intPart;
			if(f < 0.5f) return intPart;
			else return intPart + 1;
		}
		
		public void OnValueChanged(float newValue){
			int newIntValue = round(newValue);
			sliderValue = newValue;
			
			//only call OnIntValueChange when the value actually changes
			if(newIntValue != intValue){
				intValue = newIntValue;
				onIntValueChange.Invoke(intValue);
			}
		}
		
		public void SetValue(int val){
			Value = val;
		}
		
		void Start(){
			if(uiSlider == null) uiSlider = transform.GetChild(0).GetComponent<UnityEngine.UI.Slider>();
		}
		
		void Update(){
			if(snapToInt) snap();
		}
		
		/** Called every frame if we need the actual slider value to snap to closest int */
		void snap(){
			if(!Input.GetMouseButton(0)){
				if(needToRound){
					int rounded = round(uiSlider.value);
					uiSlider.value = Mathf.MoveTowards(uiSlider.value, round(uiSlider.value), snapSpeed*Time.deltaTime);
					if(rounded == uiSlider.value){
						onSnapped.Invoke();
						needToRound = false;
					}
				}
			}else{
				needToRound = true;
			}
		}
		
	}

}
