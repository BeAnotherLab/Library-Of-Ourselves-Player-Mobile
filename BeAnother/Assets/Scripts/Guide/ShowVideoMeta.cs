using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowVideoMeta : MonoBehaviour {
	
	[SerializeField] Text description;
	[SerializeField] Text objects;
	[SerializeField] Image thumbnail;
	
	void Start(){
		string videoName = CurrentSelection.Name;
		thumbnail.sprite = PngToSprite.LoadSprite(Filesystem.SDCardRoot + videoName + ".jpg");
		
		string desc;
		string obj;
		Vector3 angles;
		
		//if we have everything in playerprefs, then lets not take the meta contents
		if(AdvancedSettings.LoadCurrentSelectionFromPlayerPrefs()){
			desc = CurrentSelection.Description;
			obj = CurrentSelection.Objects;
			angles = CurrentSelection.Angles;
		}else{
			VideoMeta meta = VideoMeta.LoadMeta(videoName);
			desc = meta.description;
			obj = meta.objects;
			angles = new Vector3(meta.pitch, meta.yaw, meta.roll);
		}
		
		if(description != null)
			description.text = desc;
		if(objects != null)
			objects.text = Lang.GetText("youWillNeed") + obj;
		
		//save this for next uses
		CurrentSelection.Objects = obj;
		CurrentSelection.Description = desc;
		CurrentSelection.Angles = angles;
	}
	
}
