using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class GuideVideoPlayback : MonoBehaviour {
	
	void Start(){
		
		string path = CurrentSelection.Path;
		GetComponent<VideoPlayer>().url = path;
		GetComponent<VideoPlayer>().Prepare();
		
	}
	
}
