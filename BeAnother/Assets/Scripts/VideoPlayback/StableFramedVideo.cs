using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class StableFramedVideo : MonoBehaviour {
	
	void Awake(){
		VideoPlayer player = GetComponent<VideoPlayer>();
		if(!player.skipOnDrop){
			print("Player will now skip on drop.");
			player.skipOnDrop = true;
		}
		if(player.timeReference != VideoTimeReference.InternalTime){
			print("Player will use internal clock as reference.");
			player.timeReference = VideoTimeReference.InternalTime;
		}
	}
	
}
