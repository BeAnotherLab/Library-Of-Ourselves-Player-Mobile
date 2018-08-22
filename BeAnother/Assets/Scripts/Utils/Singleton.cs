using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {
	
	[SerializeField] string id = "default";
	
	static Dictionary<string, Singleton> map = null;
	
	public Singleton getInstance(string id){
		if(map.ContainsKey(id)) return map[id];
		else return null;
	}
	
	public void setInstance(string id, Singleton s){
		map[id] = s;
	}
	
	void Start(){
		if(map == null) map = new Dictionary<string, Singleton>();
		
		if(getInstance(id) == null){
			setInstance(id, this);
		}else{
			Destroy(gameObject);
		}
	}
	
}
