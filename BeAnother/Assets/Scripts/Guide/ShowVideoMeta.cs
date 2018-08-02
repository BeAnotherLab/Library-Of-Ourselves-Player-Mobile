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
		VideoMeta meta = VideoMeta.LoadMeta(videoName);
		description.text = meta.description;
		string o = "";
		bool firstObj = true;
		foreach(string obj in meta.objects){
			if(!firstObj)
				objects.text += ", ";
			o += obj;
			firstObj = false;
		}
		objects.text = "To perform this experience, the guide will need the following objects: " + o;
		CurrentSelection.Objects = o;//save this for next screen
		thumbnail.sprite = PngToSprite.LoadSprite(Filesystem.SDCardRoot + meta.imagePath);
	}
	
}
