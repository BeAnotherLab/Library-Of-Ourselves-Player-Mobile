using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interpolation : MonoBehaviour {
	
	[SerializeField] float speed = 1;
	[SerializeField] bool bounce = false;
	[SerializeField] bool loop = false;
	[SerializeField] float waitBetweenCycles = 0;
	[SerializeField] bool onStart = true;
	[SerializeField] bool onEnable = false;
	[SerializeField] float from = 0;
	[SerializeField] float to = 1;
	[SerializeField] EasingFunction easingFunction;
	[SerializeField] FloatEvent dynamicInterpolation;
	[SerializeField] UnityEvent onBegin;
	[SerializeField] UnityEvent onMiddle;
	[SerializeField] UnityEvent onEnd;
	
	enum EasingFunction{
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
		EaseInOutSine/*,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce*/
	}
	
	public float Speed{
		get{ return speed; }
		set{ speed = value; }
	}
	
	float val = 0;
	public float Value{
		get{ return val; }
	}
	
	bool interpolating = false;
	public bool Interpolating{
		get{ return interpolating; }
	}
	
	delegate float Ease(float input);
	Ease ease = null;
	
	void Awake(){
		ease = assignEasingFunction(easingFunction);
	}
	
	void Start(){
		if(onStart) Interpolate();
	}
	
	void OnEnable(){
		if(onEnable) Interpolate();
	}
	
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
			default: return delegate(float t){ Debug.LogError("Easing function not implemented: "+easingFunction+"."); return 0; };
		}
	}
	
	public void Interpolate(){
		Stop();
		interpolating = true;
		onBegin.Invoke();
		StartCoroutine(interpolate());
	}
	
	public void InterpolateBackward(){
		Stop();
		interpolating = true;
		onBegin.Invoke();
		StartCoroutine(interpolate(true));
	}
	
	public void Stop(){
		StopAllCoroutines();
		interpolating = false;
	}
	
	void lerp(float t){
		if(ease == null) ease = assignEasingFunction(easingFunction);
		val = Utilities.Map(0, 1, from, to, ease(t));
		dynamicInterpolation.Invoke(val);
	}
	
	IEnumerator interpolate(bool backwards = false){
		
		lerp(backwards ? 1 : 0);
		
		do{
			for(float t = 0; t<=1; t+=Time.deltaTime*speed){
				lerp(backwards ? 1 - t : t);
				yield return null;
			}
			lerp(backwards ? 0 : 1);
			
			onMiddle.Invoke();
			
			yield return new WaitForSeconds(waitBetweenCycles);
			
			if(bounce){
				for(float t = 1; t>=0; t-= Time.deltaTime*speed){
					lerp(backwards ? 1 - t : t);
					yield return null;
				}
				lerp(backwards ? 1 : 0);
				
			}
			
			onEnd.Invoke();
			
			if(bounce && loop) yield return new WaitForSeconds(waitBetweenCycles);
			
		}while(loop);
		
		interpolating = false;
		
	}
	
}
