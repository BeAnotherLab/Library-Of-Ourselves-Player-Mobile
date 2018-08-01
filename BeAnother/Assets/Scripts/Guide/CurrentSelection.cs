using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSelection : MonoBehaviour {
	
	public static CurrentSelection instance = null;
	
	[SerializeField] new string name;
	[SerializeField] string path;
	[SerializeField] bool is360;
	
	public static string Name{
		get{
			if(instance == null) return "No selection";
			return instance.name;
		}
		set{
			if(instance == null) return;
			instance.name = value;
		}
	}
	
	public static string Path{
		get{
			if(instance == null) return "No selection";
			return instance.path;
		}
		set{
			if(instance == null) return;
			instance.path = value;
		}
	}
	
	public static bool Is360{
		get{
			if(instance == null) return false;
			return instance.is360;
		}
		set{
			if(instance == null) return;
			instance.is360 = value;
		}
	}
	
	void Start(){
		DontDestroyOnLoad(this);
		if(instance == null) instance = this;
	}
	
}
