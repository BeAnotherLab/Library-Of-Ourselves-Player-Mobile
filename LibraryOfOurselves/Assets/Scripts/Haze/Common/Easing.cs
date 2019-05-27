/***

	This script was made by Jonathan Kings for use within the Haze Unity Assets.
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

///Implementation of common easing functions, including custom curves.
namespace Haze {
	[Serializable]
	public class Easing {
		
		public static readonly Easing Linear = new Easing(EasingFunction.Linear);
		
		public enum EasingFunction{
			Linear,
			EaseInQuad,
			EaseOutQuad,
			EaseInOutQuad,
			EaseInCubic,
			EaseOutCubic,
			EaseInOutCubic,
			EaseInQuart,
			EaseOutQuart,
			EaseInOutQuart,
			EaseInQuint,
			EaseOutQuint,
			EaseInOutQuint,
			EaseInSine,
			EaseOutSine,
			EaseInOutSine,
			EaseInExpo,
			EaseOutExpo,
			EaseInOutExpo,
			EaseInCirc,
			EaseOutCirc,
			EaseInOutCirc,
			EaseInElastic,
			EaseOutElastic,
			EaseInOutElastic,
			EaseInBack,
			EaseOutBack,
			EaseInOutBack,
			EaseInBounce,
			EaseOutBounce,
			EaseInOutBounce,
			CustomCurve
		}
		
		[SerializeField] EasingFunction easingFunction;
		[SerializeField] AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);//For custom definition
				
		public EasingFunction Function{
			get{ return easingFunction; }
		}
		
		delegate float Ease(float input);
		Ease ease = null;
		
		public Easing(){
			ease = assignEasingFunction(easingFunction);
		}
		
		public Easing(EasingFunction function){
			easingFunction = function;
			ease = assignEasingFunction(easingFunction);
		}
		
		///Helper functions for bounce functions
		private float __easeOutBounce(float t){
			if(t < 1/2.75f) return 7.5625f*t*t;
			else if(t < 2/2.75f) return 7.5625f*(t-=1.5f/2.75f)*t+0.75f;
			else if(t < 2.5f/2.75f) return 7.5625f*(t-=2.25f/2.75f)*t+0.9375f;
			else return 7.5625f*(t-=2.625f/2.75f)*t+0.984375f;
		}
		
		///Implementation of each easing function
		Ease assignEasingFunction(EasingFunction easingFunction){			
			switch(easingFunction){
				case EasingFunction.Linear: return delegate(float t){ return t; };
				case EasingFunction.EaseInQuad: return delegate(float t){ return t*t; };
				case EasingFunction.EaseOutQuad: return delegate(float t){ return t*(2-t); };
				case EasingFunction.EaseInOutQuad: return delegate(float t){ return t<0.5f? 2*t*t : -1+(4-2*t)*t; };
				case EasingFunction.EaseInCubic: return delegate(float t){ return t*t*t; };
				case EasingFunction.EaseOutCubic: return delegate(float t){ return (--t)*t*t+1; };
				case EasingFunction.EaseInOutCubic: return delegate(float t){ return t<0.5f? 4*t*t*t : (t-1)*(2*t-2)*(2*t-2)+1; };
				case EasingFunction.EaseInQuart: return delegate(float t){ return t*t*t*t; };
				case EasingFunction.EaseOutQuart: return delegate(float t){ return 1-(--t)*t*t*t; };
				case EasingFunction.EaseInOutQuart: return delegate(float t){ return t<0.5f? 8*t*t*t*t : 1-8*(--t)*t*t*t; };
				case EasingFunction.EaseInQuint: return delegate(float t){ return t*t*t*t*t; };
				case EasingFunction.EaseOutQuint: return delegate(float t){ return 1+(--t)*t*t*t*t; };
				case EasingFunction.EaseInOutQuint: return delegate(float t){ return t<0.5f? 16*t*t*t*t*t : 1+16*(--t)*t*t*t*t; };
				case EasingFunction.EaseInSine: return delegate(float t){ return 1-Mathf.Cos(t * Mathf.PI/2); };
				case EasingFunction.EaseOutSine: return delegate(float t){ return Mathf.Sin(t * Mathf.PI/2); };
				case EasingFunction.EaseInOutSine: return delegate(float t){ return -0.5f*Mathf.Cos(t * Mathf.PI) + 0.5f; };
				case EasingFunction.EaseInExpo: return delegate(float t){ return t == 0 ? 0 : Mathf.Pow(2,10*(t-1)); };
				case EasingFunction.EaseOutExpo: return delegate(float t){ return t == 1 ? 1 : -Mathf.Pow(2,-10*t)+1; };
				case EasingFunction.EaseInOutExpo: return delegate(float t){ t *= 2; return t == 0 ? 0 : t == 2 ? 1 : t < 1 ? 0.5f*Mathf.Pow(2,10*(t-1)) : 0.5f*(-Mathf.Pow(2,-10*(t-1))+2); };
				case EasingFunction.EaseInCirc: return delegate(float t){ return -1*(Mathf.Sqrt(1-t*t)-1); };
				case EasingFunction.EaseOutCirc: return delegate(float t){ return Mathf.Sqrt(1-(t-1)*(t-1)); };
				case EasingFunction.EaseInOutCirc: return delegate(float t){ t *= 2; return t < 1 ? -0.5f*(Mathf.Sqrt(1-t*t)-1) : 0.5f*(Mathf.Sqrt(1-(t-2)*(t-2))+1); };
				case EasingFunction.EaseInElastic: return delegate(float t){ return t == 0 ? 0 : t == 1 ? 1 : -(Mathf.Pow(2,10*(t-=1))*Mathf.Sin((t-0.075f)*(2*Mathf.PI)/0.3f)); };
				case EasingFunction.EaseOutElastic: return delegate(float t){ return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10*t) * Mathf.Sin((t-0.075f)*(2*Mathf.PI)/0.3f) + 1; };
				case EasingFunction.EaseInOutElastic: return delegate(float t){ t *= 2; return t == 0 ? 0 : t == 2 ? 1 : t < 1 ? -.5f*(Mathf.Pow(2,10*(t-=1))*Mathf.Sin((t-0.075f)*(2*Mathf.PI)/0.45f)) : Mathf.Pow(2,-10*(t-=1))*Mathf.Sin((t-0.075f)*(2*Mathf.PI)/0.45f)*.5f+1; };
				case EasingFunction.EaseInBack: return delegate(float t){ return t*t*(2.70158f*t-1.70158f); };
				case EasingFunction.EaseOutBack: return delegate(float t){ return (t-=1)*t*(2.70158f*t+1.70158f)+1; };
				case EasingFunction.EaseInOutBack: return delegate(float t){ t *= 2; return t < 1 ? 0.5f*(t*t*(3.5949095f*t-2.5949095f)) : 0.5f*((t-=2)*t*(3.5949095f*t+2.5949095f)+2); };
				case EasingFunction.EaseInBounce: return delegate(float t){ return 1-__easeOutBounce(1-t); };
				case EasingFunction.EaseOutBounce: return delegate(float t){ return __easeOutBounce(t); };
				case EasingFunction.EaseInOutBounce: return delegate(float t){ return t < 0.5f ? 0.5f*(1-__easeOutBounce(1-t*2)) : 0.5f*__easeOutBounce(t*2-1)+0.5f; };
				case EasingFunction.CustomCurve: return delegate(float t){ return curve.Evaluate(t); };
				default: return delegate(float t){ Debug.LogError("Easing function not implemented: "+easingFunction+"."); return 0; };
			}
		}
		
		///Use like this:
		/// easing.T = 0.5f; (0..1)
		/// float t = easing.T; (0..1)
		public float T{
			set{ 
				#if UNITY_EDITOR
				if(ease == null){
					ease = assignEasingFunction(easingFunction);
				}
				#endif
				_t = ease(Mathf.Clamp01(value)); }
			get{
				return _t;
			}
		}
		float _t = 0;
		
		///Use like this:
		/// float t = easing.Get(0.5f);
		public float Get(float t){
			T = t;
			return T;
		}
		
		#if UNITY_EDITOR
		public void __clearEase(){
			ease = null;
		}
		#endif
		
	}
	
	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(Easing))]
	public class EasingDrawer : PropertyDrawer{
		
		float testValue = 0;
		bool showField = false;
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
			return 0;
		}
		
		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label){
			
			showField = EditorGUILayout.Foldout(showField, "Easing");
			if(!showField) return;
			
			//Get the target object
			var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
			Easing easing = obj as Easing;
			if (obj.GetType().IsArray){
				int index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
				easing = ((Easing[])obj)[index];
			}
			
			//Display enum popup
			SerializedProperty functionProperty = property.FindPropertyRelative("easingFunction");
			EditorGUILayout.PropertyField(functionProperty);
			
			//Display animation curve if needed
			if(easing.Function == Easing.EasingFunction.CustomCurve){
				EditorGUILayout.BeginHorizontal();
				SerializedProperty curveProperty = property.FindPropertyRelative("curve");
				EditorGUILayout.PropertyField(curveProperty);
				if(GUILayout.Button("Reset curve"))
					curveProperty.animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
				EditorGUILayout.EndHorizontal();
			}
			
			//Display example slider.
			easing.__clearEase();
			testValue = EditorGUILayout.Slider(testValue, 0, 1);
			easing.T = testValue;
			EditorGUILayout.Slider(easing.T, -0.5f, 1.5f);
			
		}
	}
	#endif
}
