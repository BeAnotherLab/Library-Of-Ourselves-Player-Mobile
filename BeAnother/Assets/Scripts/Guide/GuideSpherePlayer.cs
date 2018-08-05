using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class GuideSpherePlayer : MonoBehaviour {
	
	[SerializeField] VideoPlayer semispherePlayer;
	[SerializeField] VideoPlayer spherePlayer;
	
	VideoPlayer vp;
	
	void Start(){
		string path = Filesystem.SDCardRoot + CurrentSelection.Name + ".mp4";
		if(File.Exists(path)){
			if(CurrentSelection.Is360){
				vp = spherePlayer;
				semispherePlayer.gameObject.SetActive(false);
			}else{
				vp = semispherePlayer;
				spherePlayer.gameObject.SetActive(false);
			}
			vp.url = path;
			vp.Prepare();
		}
	}
	
	public void Play(){
		if(vp != null)vp.Play();
	}
	
	public void Pause(){
		if(vp != null)vp.Pause();
	}
	
	public void Stop(){
		if(vp != null)vp.Stop();
	}
	
}
