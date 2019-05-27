using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {
	
	[SerializeField] string id = "default";
	
	static Dictionary<string, Singleton> map = null;
	
	public static Singleton GetInstance(string id){
		if(map.ContainsKey(id)) return map[id];
		else return null;
	}
	
	public static void SetInstance(string id, Singleton s){
		map[id] = s;
	}
	
	public static void RemoveInstance(string id){
		Singleton instance = GetInstance(id);
		if(instance != null){
			Destroy(instance.gameObject);
			map.Remove(id);
		}
	}
	
	void Start(){
		if(map == null) map = new Dictionary<string, Singleton>();
		
		if(GetInstance(id) == null){
			SetInstance(id, this);
		}else{
			Destroy(gameObject);
		}
	}
	
}
