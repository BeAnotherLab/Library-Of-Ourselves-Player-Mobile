using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AdvancedSettings : MonoBehaviour {
	
	[SerializeField] InputField description;
	[SerializeField] InputField objects;
	[SerializeField] InputField pitch;
	[SerializeField] InputField yaw;
	[SerializeField] InputField roll;
	
	bool saved = true;//no changes yet
	
	void Start(){
		description.text = CurrentSelection.Description;
		objects.text = CurrentSelection.Objects;
		pitch.text = ""+CurrentSelection.Angles.x;
		yaw.text = ""+CurrentSelection.Angles.y;
		roll.text = ""+CurrentSelection.Angles.z;
	}
	
	float f(string s){
		return (float)Convert.ToDouble(s);
	}
	
	public void OnEndEditDescription(string d){
		CurrentSelection.Description = d;
		saved = false;
	}
	
	public void OnEndEditObjects(string o){
		CurrentSelection.Objects = o;
		saved = false;
	}
	
	public void OnEndEditPitch(string p){
		Vector3 angles = CurrentSelection.Angles;
		angles.x = f(p);
		CurrentSelection.Angles = angles;
		saved = false;
	}
	
	public void OnEndEditYaw(string y){
		Vector3 angles = CurrentSelection.Angles;
		angles.y = f(y);
		CurrentSelection.Angles = angles;
		saved = false;
	}
	
	public void OnEndEditRoll(string r){
		Vector3 angles = CurrentSelection.Angles;
		angles.z = f(r);
		CurrentSelection.Angles = angles;
		saved = false;
	}
	
	public void OnSave(){
		//if json file does not exist, save in meta, otherwise just playerprefs
		if(saved) return;
		if(VideoMeta.HasMeta(CurrentSelection.Name)){
			//save current selection in playerprefs
			PlayerPrefs.SetString(CurrentSelection.Name + "d", CurrentSelection.Description);
			PlayerPrefs.SetString(CurrentSelection.Name + "o", CurrentSelection.Objects);
			PlayerPrefs.SetFloat(CurrentSelection.Name + "p", CurrentSelection.Angles.x);
			PlayerPrefs.SetFloat(CurrentSelection.Name + "y", CurrentSelection.Angles.y);
			PlayerPrefs.SetFloat(CurrentSelection.Name + "r", CurrentSelection.Angles.z);
		}else{
			//save in meta
			VideoMeta vm = VideoMeta.LoadMeta(CurrentSelection.Name);
			vm.description = CurrentSelection.Description;
			vm.objects = CurrentSelection.Objects;
			vm.pitch = CurrentSelection.Angles.x;
			vm.yaw = CurrentSelection.Angles.y;
			vm.roll = CurrentSelection.Angles.z;
			VideoMeta.SaveMeta(CurrentSelection.Name, vm);
		}
		saved = true;
	}
	
	public void OnReset(){
		VideoMeta vm = VideoMeta.LoadMeta(CurrentSelection.Name);
		CurrentSelection.Description = vm.description;
		CurrentSelection.Objects = vm.objects;
		CurrentSelection.Angles = new Vector3(vm.pitch, vm.yaw, vm.roll);
		OnSave();//save those reset values back to playerprefs
		
		description.text = CurrentSelection.Description;
		objects.text = CurrentSelection.Objects;
		pitch.text = ""+CurrentSelection.Angles.x;
		yaw.text = ""+CurrentSelection.Angles.y;
		roll.text = ""+CurrentSelection.Angles.z;
		
		saved = true;
	}
	
	public static bool LoadCurrentSelectionFromPlayerPrefs(){
		if(PlayerPrefs.HasKey(CurrentSelection.Name+"d")){
			CurrentSelection.Description = PlayerPrefs.GetString(CurrentSelection.Name + "d");
			CurrentSelection.Objects = PlayerPrefs.GetString(CurrentSelection.Name + "o");
			Vector3 angles = new Vector3();
			angles.x = PlayerPrefs.GetFloat(CurrentSelection.Name + "p");
			angles.y = PlayerPrefs.GetFloat(CurrentSelection.Name + "y");
			angles.z = PlayerPrefs.GetFloat(CurrentSelection.Name + "r");
			CurrentSelection.Angles = angles;
			return true;
		}else return false;
	}
	
	public bool isSaved(){
		return saved;
	}
	
}
