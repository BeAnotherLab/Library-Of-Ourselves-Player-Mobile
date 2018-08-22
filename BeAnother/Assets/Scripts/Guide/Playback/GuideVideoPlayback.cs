using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

[RequireComponent(typeof(VideoPlayer))]
public class GuideVideoPlayback : MonoBehaviour {
	
	[SerializeField] UnityEvent onVideoEnd;
	
	void Start(){
		
		VideoPlayer player = GetComponent<VideoPlayer>();
		
		string path = CurrentSelection.Path;
		player.url = path;
		player.Prepare();
		
		player.loopPointReached += delegate{ onVideoEnd.Invoke(); };
		
	}
	
}
