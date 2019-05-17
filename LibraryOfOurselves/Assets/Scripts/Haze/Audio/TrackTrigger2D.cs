using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrigger2D : MonoBehaviour {
	
	[SerializeField] List<Track> affectedTracks;
	[SerializeField] float fadeTime = 0.1f;
	
	[Serializable]
	class Track{
		[SerializeField] string name = "";
		[SerializeField] bool startWhenGoingRight = true;//otherwise, will start when going left
		
		int trackId = -1;
		int TrackId{
			get{
				if(trackId == -1) trackId = MultitrackAudioSource.instance.GetTrackId(name);
				return trackId;
			}
		}
		
		void setVolume(float v, float fadeTime){
			MultitrackAudioSource.instance.FadeTrack(TrackId, v, fadeTime);
		}
		
		public void onGoLeft(float fadeTime){
			if(startWhenGoingRight){
				setVolume(0, fadeTime);
			}else{
				setVolume(1, fadeTime);
			}
		}
		
		public void onGoRight(float fadeTime){
			if(startWhenGoingRight){
				setVolume(1, fadeTime);
			}else{
				setVolume(0, fadeTime);
			}
		}
	}
	
	void Awake(){
		//create two child gameobjects
		GameObject left = new GameObject(name + " trigger left");
		left.transform.SetParent(transform, false);
		BoxCollider2D colliderLeft = left.AddComponent<BoxCollider2D>();
		colliderLeft.offset = new Vector2(-0.25f, 0.0f);
		colliderLeft.size = new Vector2(0.5f, 1.0f);
		colliderLeft.isTrigger = true;
		Trigger2D leftTrigger = left.AddComponent<Trigger2D>();
		leftTrigger.onEnter.AddListener(delegate{
			foreach(Track t in affectedTracks){
				t.onGoLeft(fadeTime);
			}
		});
		
		GameObject right = new GameObject(name + " trigger right");
		right.transform.SetParent(transform, false);
		BoxCollider2D colliderRight = right.AddComponent<BoxCollider2D>();
		colliderRight.offset = new Vector2(0.25f, 0.0f);
		colliderRight.size = new Vector2(0.5f, 1.0f);
		colliderRight.isTrigger = true;
		Trigger2D rightTrigger = right.AddComponent<Trigger2D>();
		rightTrigger.onEnter.AddListener(delegate{
			foreach(Track t in affectedTracks){
				t.onGoRight(fadeTime);
			}
		});
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmos(){
		
		Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
		Vector3 halfRight = transform.right*0.5f*transform.lossyScale.x;
		Vector3 halfUp = transform.up*0.5f*transform.lossyScale.y;
		Gizmos.DrawRay(transform.position-halfRight-halfUp, halfUp*2.0f);
		Gizmos.DrawRay(transform.position+halfRight-halfUp, halfUp*2.0f);
		Gizmos.DrawRay(transform.position-halfRight+halfUp, halfRight*2.0f);
		Gizmos.DrawRay(transform.position-halfRight-halfUp, halfRight*2.0f);
		DrawArrow(transform.position-halfRight, halfRight*2.0f);
		
	}
	
	// Based on  https://forum.unity3d.com/threads/debug-drawarrow.85980/
	static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1.0f){
		Gizmos.DrawRay(pos, direction);
		DrawArrowEnd(pos, direction, arrowHeadLength, arrowHeadAngle, arrowPosition);
    }
	static void DrawArrowEnd(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 1.0f){
        Vector3 right = (Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
        Vector3 left = (Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
        Vector3 up = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
        Vector3 down = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
 
        Vector3 arrowTip = pos + (direction*arrowPosition);
		
        Gizmos.DrawRay(arrowTip, right);
        Gizmos.DrawRay(arrowTip, left);
        Gizmos.DrawRay(arrowTip, up);
        Gizmos.DrawRay(arrowTip, down);
    }
	#endif
	
}
