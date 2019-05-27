using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFloat : MonoBehaviour {
	
	[SerializeField] Function function = Function.Square;
	[SerializeField] FloatEvent passSmoothed;
	
	public enum Function{
		None, Square, InvertSquare, Root
	}
	
	abstract class SmoothFunction{
		public abstract float smooth(float f);
	}
	
	class NoFunction : SmoothFunction{
		public override float smooth(float f){ return f; }
	}
	
	class SquareFunction : SmoothFunction{
		bool invert = false;
		public SquareFunction(bool invert){ this.invert = invert; }
		public override float smooth(float f){ if(invert) return 1-f*f; else return f*f; }
	}
	
	class RootFunction : SmoothFunction{
		public override float smooth(float f){ return Mathf.Sqrt(f); }
	}
	
	SmoothFunction func;
	
	void Start(){
		switch(function){
			case Function.None: func = new NoFunction(); break;
			case Function.Square: func = new SquareFunction(false); break;
			case Function.InvertSquare: func = new SquareFunction(true); break;
			case Function.Root : func = new RootFunction(); break;
		}
	}
	
	public void Smooth(float f){//input is [0-1], output [0,1]
		
		f = func.smooth(f);
		
		passSmoothed.Invoke(f);
	}
	
}
