using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioRandomizer : MonoBehaviour {
	
	[SerializeField] bool changePitch = false;
	[SerializeField] bool changePitchWhenRestarting = false;
	[SerializeField] float minPitch = 0.8f;
	[SerializeField] float maxPitch = 3.0f;
	[SerializeField] float pitchAccel = 0.3f;
	[SerializeField] bool playAtRandom = false;
	[SerializeField] float maxTimeUntilPlay = 5.0f;
	[SerializeField] bool randomClip = false;
	[SerializeField] List<AudioClip> clips;
	[SerializeField] bool playOnStart = false;
	
	AudioSource source = null;
	
	float pitchVelocity;
	float nextPlay;
	
	void Start(){
		check();
		if(playOnStart) Play();
	}
	
	void check(){
		if(source != null) return;
		source = GetComponent<AudioSource>();
		if(changePitch){
			source.pitch = Random.Range(minPitch, maxPitch);
			pitchVelocity = Random.Range(-pitchAccel, pitchAccel);
		}
		if(playAtRandom)
			nextPlay = Random.Range(0.0f, maxTimeUntilPlay);
		randomAudioClip();
	}
	
	void randomAudioClip(){
		if(!randomClip) return;
		source.clip = clips[Random.Range(0, clips.Count)];
	}
	
	public void Play(){
		check();
		randomAudioClip();
		if(changePitchWhenRestarting){
			source.pitch = Random.Range(minPitch, maxPitch);
		}
		source.Play();
	}
	
	void Update(){
		if(source.isPlaying){
			
			if(changePitch){
				//update pitch:
				pitchVelocity += Random.Range(-pitchAccel, pitchAccel) * Time.deltaTime;
				pitchVelocity = Mathf.Max(Mathf.Min(pitchVelocity, pitchAccel), -pitchAccel);
				source.pitch += pitchVelocity * Time.deltaTime;
				if(source.pitch < minPitch){
					source.pitch = minPitch;
					pitchVelocity = -pitchVelocity;
				}else if(source.pitch > maxPitch){
					source.pitch = maxPitch;
					pitchVelocity = -pitchVelocity;
				}
			}
			
		}else if(playAtRandom){
			
			//start playing in a certain amount of time
			nextPlay -= Time.deltaTime;
			if(nextPlay < 0){
				randomAudioClip();
				source.Play();
				nextPlay = Random.Range(0.0f, maxTimeUntilPlay);
				
				if(changePitchWhenRestarting){
					source.pitch = Random.Range(minPitch, maxPitch);
				}
				
			}
			
		}
	}
	
}
