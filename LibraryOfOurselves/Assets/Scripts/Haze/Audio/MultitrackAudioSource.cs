using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultitrackAudioSource : MonoBehaviour {
	
	public static MultitrackAudioSource instance = null;
	
	[SerializeField] List<Track> tracks;
	
	List<AudioSource> sources = new List<AudioSource>();
	
	[Serializable]
	class Track{
		public string name = "";
		public AudioClip clip = null;
		public bool playOnAwake = true;
		[HideInInspector] public Coroutine currentCoroutine = null;
	}
	
	void Awake(){
		for(int i = 0; i<tracks.Count; ++i){
			GameObject go = new GameObject();
			go.name = tracks[i].name;
			go.transform.SetParent(transform);
			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = tracks[i].clip;
			source.bypassEffects = true;
			source.bypassReverbZones = true;
			source.loop = true;
			source.volume = tracks[i].playOnAwake ? 1.0f : 0.0f;
			source.spatialize = false;
			source.Play();
			sources.Add(source);
		}
	}
	
	void OnEnable(){
		if(instance != null){
			Debug.LogError("You can only have one instance of MultitrackAudioSource activeAtOnce.");
			enabled = false;
			return;
		}
		instance = this;
	}
	
	void OnDisable(){
		if(instance == this) instance = null;
	}
	
	public void Play(){
		foreach(AudioSource source in sources)
			source.Play();
	}
	
	public void Pause(){
		foreach(AudioSource source in sources)
			source.Pause();
	}
	
	public void Stop(){
		foreach(AudioSource source in sources)
			source.Stop();
	}
	
	public float GetTrackVolume(int trackId){
		if(!checkTrack(trackId)) return 0;
		
		return sources[trackId].volume;
	}
	
	public float GetTrackVolume(string trackName){
		return GetTrackVolume(GetTrackId(trackName));
	}
	
	public void SetTrackVolume(int trackId, float volume){
		if(!checkTrack(trackId)) return;
		
		sources[trackId].volume = volume;
		
	}
	
	public void SetTrackVolume(string trackName, float volume){
		SetTrackVolume(GetTrackId(trackName), volume);
	}
	
	public void FadeTrack(int trackId, float targetVolume, float time = 0.1f){
		if(!checkTrack(trackId)) return;
		
		if(tracks[trackId].currentCoroutine != null) StopCoroutine(tracks[trackId].currentCoroutine);
		tracks[trackId].currentCoroutine = StartCoroutine(fadeTrack(trackId, targetVolume, time));
	}
	
	public void FadeTrack(string trackName, float targetVolume, float time = 0.1f){
		FadeTrack(GetTrackId(trackName), targetVolume, time);
	}
	
	IEnumerator fadeTrack(int trackId, float targetVolume, float time){
		
		float inverseTime = 1.0f/time;
		float initialVolume = GetTrackVolume(trackId);
		for(float t = 0; t<1; t+=Time.unscaledDeltaTime * inverseTime){
			SetTrackVolume(trackId, Mathf.Lerp(initialVolume, targetVolume, t));
			yield return null;
		}
		SetTrackVolume(trackId, targetVolume);
		
		tracks[trackId].currentCoroutine = null;
	}
	
	public int GetTrackId(string trackName){
		for(int i = 0; i<tracks.Count; ++i){
			if(tracks[i].name == trackName) return i;
		}
		Debug.LogWarning("No such track as " + trackName + " on MultitrackAudioSource " + name);
		return -1;
	}
	
	bool checkTrack(int trackId){
		if(trackId <= -1 || trackId >= tracks.Count){
			Debug.LogError("Requested wrong track id or track name.");
			return false;
		}
		return true;
	}
	
}
