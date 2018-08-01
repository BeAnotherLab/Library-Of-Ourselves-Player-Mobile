using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuUIVideo : MonoBehaviour {
	
	[SerializeField] Text nameDisplay;
	[SerializeField] UnityEvent onSelect;
	
	new string name;
	string path;
	bool is360;
	
	public void Init(string name, string path, bool is360){
		nameDisplay.text = name;
		this.name = name;
		this.path = path;
		this.is360 = is360;
	}
	
	/** Call when user selects this video */
	public void OnSelect(){
		print("Selecting: " + name + " (" + path + ")");
		CurrentSelection.Name = name;
		CurrentSelection.Path = path;
		CurrentSelection.Is360 = is360;
		onSelect.Invoke();
	}
	
}
