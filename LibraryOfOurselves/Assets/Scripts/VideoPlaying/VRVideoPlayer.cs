﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class VRVideoPlayer : MonoBehaviour{

	[SerializeField] float timeBetweenSyncs = 0.75f;
	[SerializeField] private GameObject _semispherePlayer;
	[SerializeField] private RenderTexture _semiSphereRenderTexture;
	[SerializeField] private GameObject _spherePlayer;
	[SerializeField] private RenderTexture _sphereRenderTexture;
	[SerializeField] private Material _renderMaterial;
	
	[SerializeField] AudioSource leftAudio;
	[SerializeField] AudioSource rightAudio;
	[SerializeField] UnityEvent onVideoEnds;
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onPlay;
	[SerializeField] GameObject choiceContainer;
	
	[SerializeField] private GameObject optionMeshPrefab;
	[SerializeField] private Transform optionsParent;
	
	[SerializeField] GameObject blackScreen;
	[SerializeField] Transform rotator;
	[SerializeField] AudioLoadingMode audioLoadingMode = AudioLoadingMode.UnityWebResource;
	[SerializeField] GameObject gvrControllerInput;

	[Header("Sync settings")]
	[SerializeField] float allowedErrorForSyncedPlayback = 0.5f;
	[SerializeField] float maximumAllowedErrorBeforeResync = 3.0f;
	[SerializeField] float maximumPlaybackSpeed = 1.5f;
	[SerializeField] float minimumPlaybackSpeed = 0.75f;

	[SerializeField] private int _frameBeforeEndInSeconds;
	public enum AudioLoadingMode {
		WWW,
		NLayer,
		UnityWebResource
	}

	public static VRVideoPlayer Instance { get; private set; }

	MediaPlayer player;
	Camera mainCamera;
	bool errorWhileLoading = false;
	float firstTap = 0;
	bool is360;//whether the current video is 360 degrees
	bool hasVideoPlaying = false;

	bool __binauralAudio = false;
	bool BinauralAudio { 
		get { return __binauralAudio; }
		set {
			__binauralAudio = value;
			leftAudio.enabled = value;
			rightAudio.enabled = value;
			//player.audioOutputMode = value ? VideoAudioOutputMode.None : VideoAudioOutputMode.Direct;
		}
	}

	double VideoTime {
		get { return player.Control.GetCurrentTime(); }
		set
		{
			player.Control.Seek(value);
			if(BinauralAudio) {
				leftAudio.time = (float)value;
				rightAudio.time = (float)value;
			}
		}
	}

	float PlaybackSpeed {
		set {
			player.Control.SetPlaybackRate(value);
			if(BinauralAudio) {
				leftAudio.pitch = value;
				rightAudio.pitch = value;
			}
		}
	}

	public struct VideoLoadingResponse { public bool ok;public string errorMessage; }

	long lastReadyFrame = -1;

	private bool _LoadingForChoicePosition;
	
	private void OnEnable()
	{
		player = GetComponent<MediaPlayer>();
		player.Events.AddListener(MediaPlayerEventReceived);
		VRGazeChoice.ConfirmVRGazeSelection += OnSelectOption;
	}

	private void Start() {
		Camera.main.backgroundColor = DeviceColour.getDeviceColor(SystemInfo.deviceUniqueIdentifier);//Set background colour to something unique
		Instance = this;
		mainCamera = Camera.main;

		choiceContainer.SetActive(false);
		blackScreen.SetActive(false);

		BinauralAudio = false;

		/*
		player.errorReceived += delegate (VideoPlayer player, string message) {
			Haze.Logger.LogError("VideoPlayer error: " + message);
			errorWhileLoading = true;
			endOfVideo();
		};
		*/

		
	}

	private void OnDestroy() {
		Instance = null;
	}

	static string getAudioPath(string videoName, bool left) {
		return Application.persistentDataPath + videoName + "-" + (left ? "l" : "r"); //extension will be assumed .wav unless the wave file doesn't exist in which case .mp3 will be selected
	}

	private void MediaPlayerEventReceived(MediaPlayer player, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
	{
		switch (eventType)
		{
			case MediaPlayerEvent.EventType.FinishedPlaying:
				//play();
				break;
			case MediaPlayerEvent.EventType.FirstFrameReady:
				Debug.Log("first frame ready");
				lastReadyFrame = player.Control.GetCurrentTimeFrames();
				if (_LoadingForChoicePosition)
				{
					StartCoroutine(SeekToEnd());
					_LoadingForChoicePosition = false;
				}
				break;
		}
	}

	private IEnumerator SeekToEnd()
	{
		yield return new WaitForSeconds(2);
		Debug.Log("first fram ready, seeking to end");
		player.Control.Seek(player.Info.GetDuration() - _frameBeforeEndInSeconds);
	}
	
	public static bool IsVideoAvailable(string videoName) {
		string path = Path.Combine(DataFolder.Path, videoName + ".mp4"); 
		Haze.Logger.Log("Checking if we have " + path);
		return File.Exists( path);
	}

	public async Task<VideoLoadingResponse> LoadVideo(string videoName, string mode) { //TODO async not actually necessary?
		VideoLoadingResponse response = new VideoLoadingResponse();
		response.ok = true;
		response.errorMessage = "";

		Haze.Logger.Log("Loading video " + videoName + "...");
		errorWhileLoading = false;

		if (!IsVideoAvailable(videoName)) 
		{
			response.errorMessage = "Video unavailable.";
			response.ok = false;
			Haze.Logger.LogError("Video " + videoName + " could not be found.");
			return response;
		}

		is360 = false;
		if (mode.Length >= 3 && mode[0] == '3' && mode[1] == '6' && mode[2] == '0') 
		{
			is360 = true;
			Haze.Logger.Log("Loading 360 degree video.");
		}
		else Haze.Logger.Log("Loading 235 degree video.");

		//Check if we need to play binaural audio
		string leftAudioFile = getAudioPath(videoName, true);
		string rightAudioFile = getAudioPath(videoName, false);
		
		//determine extension based on whether the Wave file exists otherwise fallback onto mp3
		if(File.Exists(leftAudioFile + ".wav")) leftAudioFile += ".wav"; else leftAudioFile += ".mp3";
		if(File.Exists(rightAudioFile + ".wav")) rightAudioFile += ".wav"; else rightAudioFile += ".mp3";

		//resources needed for loading audio
		WWW leftWWW = null;
		WWW rightWWW = null;
		AudioClip leftClip = null;
		AudioClip rightClip = null;
		UnityWebRequest leftuwr = null;
		UnityWebRequest rightuwr = null;

		if (File.Exists(leftAudioFile) && File.Exists(rightAudioFile)) 
		{
			BinauralAudio = true;

			//load the sound into the audio sources
			switch (audioLoadingMode) 
			{
				case AudioLoadingMode.WWW:
					leftWWW = new WWW("file://" + leftAudioFile.Replace('\\', '/'));
					rightWWW = new WWW("file://" + rightAudioFile.Replace('\\', '/'));
					break;
				case AudioLoadingMode.NLayer:
					leftClip = NLayerLoader.LoadMp3(leftAudioFile);
					rightClip = NLayerLoader.LoadMp3(rightAudioFile);
					break;
				case AudioLoadingMode.UnityWebResource:
					leftuwr = UnityWebRequestMultimedia.GetAudioClip("file://" + leftAudioFile.Replace('\\', '/'), AudioType.UNKNOWN);
					rightuwr = UnityWebRequestMultimedia.GetAudioClip("file://" + rightAudioFile.Replace('\\', '/'), AudioType.UNKNOWN);
					leftuwr.Send();
					rightuwr.Send();
					break;
			}
		} 
		else 
		{
			BinauralAudio = false;
		}
		Haze.Logger.Log("Binaural audio: " + (BinauralAudio ? "on" : "off"));
		if (BinauralAudio) 
		{
			Haze.Logger.Log("Loading binaural audio files: " + leftAudioFile + " and " + rightAudioFile);
		}
		
		//Prepare video player
		player.OpenMedia(new MediaPath(Path.Combine(DataFolder.Path, videoName + ".mp4"), MediaPathType.AbsolutePathOrURL), autoPlay:false);
		
		PlaybackSpeed = 1;
		lastReadyFrame = -1; //TODO delete, value never used

		//Load the video into the player...
		DateTime before = DateTime.Now;
		TimeSpan took = DateTime.Now - before;
		Haze.Logger.Log("Player is prepared! Took: " + took.TotalMilliseconds + " ms.");

		//Wait for the audio sources to load
		if (BinauralAudio) 
		{
			Haze.Logger.Log("Waiting for audio files to load...");

			switch (audioLoadingMode) 
			{
				case AudioLoadingMode.WWW:
					while(!leftWWW.isDone || !rightWWW.isDone)
						await Task.Delay(20);
					leftAudio.clip = leftWWW.GetAudioClip();
					rightAudio.clip = rightWWW.GetAudioClip();
					while(leftAudio.clip.loadState == AudioDataLoadState.Loading || rightAudio.clip.loadState == AudioDataLoadState.Loading)
						await Task.Delay(20);
					break;
				case AudioLoadingMode.NLayer:
					while(leftClip.loadState == AudioDataLoadState.Loading || rightClip.loadState == AudioDataLoadState.Loading)
						await Task.Delay(20);
					leftAudio.clip = leftClip;
					rightAudio.clip = rightClip;
					break;
				case AudioLoadingMode.UnityWebResource:
					while(!leftuwr.isDone || !rightuwr.isDone)
						await Task.Delay(20);
					leftAudio.clip = DownloadHandlerAudioClip.GetContent(leftuwr);
					rightAudio.clip = DownloadHandlerAudioClip.GetContent(rightuwr);
					if(leftuwr.isNetworkError)
						Haze.Logger.LogError("Could not open audio file " + leftAudioFile + ": " + leftuwr.error);
					if(rightuwr.isNetworkError)
						Haze.Logger.LogError("Could not open audio file " + rightAudioFile + ": " + rightuwr.error);
					break;
			}

			Haze.Logger.Log("Audio loaded: Left: " + leftAudio.clip.loadState + " / Right: " + rightAudio.clip.loadState);

		}

		Reorient(Vector3.zero);//reset orientation

		//Show first frame as soon as it's loaded in and rendered, making sure the video is 100% paused when we do so.
		float previousVolume = Volume;
		blackScreen.SetActive(false);
		if (is360)
		{
			GetComponent<ResolveToRenderTexture>().ExternalTexture = _sphereRenderTexture;
			_renderMaterial.mainTexture = _sphereRenderTexture;
			_spherePlayer.SetActive(true);
		}
		else
		{
			GetComponent<ResolveToRenderTexture>().ExternalTexture = _semiSphereRenderTexture;
			_renderMaterial.mainTexture = _semiSphereRenderTexture;
			_semispherePlayer.SetActive(true);
		}

		return response;
	}

	void play() {
		player.Play();
		if(BinauralAudio) {
			leftAudio.Play();
			rightAudio.Play();
		}
		hasVideoPlaying = true;
	}

	void pausePlayback() {
		player.Pause();
		if(BinauralAudio) {
			leftAudio.Pause();
			rightAudio.Pause();
		}
	}

	void stop() {
		player.Stop();
		if(BinauralAudio) {
			leftAudio.Stop();
			rightAudio.Stop();
		}
		hasVideoPlaying = false;
	}

	float Volume {
		get {
			if(BinauralAudio)
				return leftAudio.volume;
			/*
			else if(player.audioTrackCount > 0)
				return player.GetDirectAudioVolume(0);*/
			else return 0;
		}
		set {
			if(BinauralAudio) {
				leftAudio.volume = value;
				rightAudio.volume = value;
			} else {
				//for(ushort track = 0; track < player.AudioTracks; ++track) {
				player.AudioVolume = value;
			}
		}
	}

	public void PlayVideo(DateTime timestamp, float syncTime, Vector3 settings) {

		blackScreen.SetActive(false);

		//figure out difference in time between now and timestamp
		TimeSpan difference = DateTime.Now - timestamp;
		Haze.Logger.Log("Started playing video " + difference.TotalSeconds + " s ago.");
		Haze.Logger.Log("Using sync time = " + syncTime);

		//Apply sync time settings sent by Guide
		timeBetweenSyncs = syncTime;
		allowedErrorForSyncedPlayback = settings.x;
		maximumAllowedErrorBeforeResync = settings.y;
		maximumPlaybackSpeed = settings.z;
		minimumPlaybackSpeed = 1.0f / settings.z;

		VideoTime = 0;
		play();
		
		if(is360) _spherePlayer.SetActive(true);
		else _semispherePlayer.SetActive(true);

		onPlay.Invoke();

		SendSyncMessages();
		sendImmediateSync();
	}

	public void Sync(DateTime unused, double videoTime) {
		//Assume at timestamp it was at videoTime; if it would've been later, slow down time slightly; if it would've been earlier, speed up time slightly
		float targetTime = (float)videoTime;
		float actualTime = (float)player.Control.GetCurrentTime();
		float delta = actualTime - targetTime;//Negative->go faster; Positive->go slower

		//Shall we speed up or slow down?
		if(Mathf.Abs(delta) < allowedErrorForSyncedPlayback) {
			PlaybackSpeed = 1;
		}else if(Mathf.Abs(delta) > maximumAllowedErrorBeforeResync) {//too much difference, let's just pop back to the right point
			Haze.Logger.Log("Target time = " + targetTime + " / Actual time = " + actualTime + " // Difference = " + delta + " ==> Too much difference, jumping to " + targetTime);
			VideoTime = targetTime;
			PlaybackSpeed = 1;
		}else if(delta < 0) {// actualTime < targetTime -> go faster
			delta = Mathf.Abs(delta) - allowedErrorForSyncedPlayback;//0 when the difference is the allowed range
			//remap delta to 0..1
			delta = Utilities.Map(0, maximumAllowedErrorBeforeResync - allowedErrorForSyncedPlayback, 0, 1, delta);
			//and remap from 0..1 to 1..max playback speed
			PlaybackSpeed = Utilities.Map(0, 1, 1, maximumPlaybackSpeed, delta);
		} else {// actualTime > targetTime -> go slower
			delta = delta - allowedErrorForSyncedPlayback;//0 when difference is 0.5, 1 at 1.5 and higher
			//remap delta to 0..1
			delta = Utilities.Map(0, maximumAllowedErrorBeforeResync - allowedErrorForSyncedPlayback, 0, 1, delta);
			//and remap from 0..1 to 1..min playback speed
			PlaybackSpeed = Utilities.Map(0, 1, 1, minimumPlaybackSpeed, delta);
		}

		if(!player.Control.IsPlaying()) {
			Haze.Logger.LogWarning("Player was stopped when we received Sync message");
			play();
			VideoTime = targetTime;
		}

		//Respond
		sendImmediateSync();
	}

	//Toggles between playing and paused.
	public async void PauseVideo(double videoTime, bool pause) {
		if(!pause) {
			VideoTime = videoTime;
			PlaybackSpeed = 1;
			play();
			VideoTime = videoTime;
			onPlay.Invoke();
		} else {
			VideoTime = videoTime;
			onPause.Invoke();
			await Task.Delay(250);//give it some time to catch up before we trigger the Pause
			VideoTime = videoTime;
			pausePlayback();
		}
		sendImmediateSync();
	}

	public void StopVideo() {
		endOfVideo();
	}

	public void Recenter() {
		if(mainCamera == null) mainCamera = Camera.main;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
	}

	async void SendSyncMessages() {
		while(hasVideoPlaying && VRAdapter.Instance) {
			if(player.Control.IsPlaying()) {
				sendImmediateSync();
			}
			await Task.Delay((int)(timeBetweenSyncs * 1000));
		}
	}

	void sendImmediateSync() {
		VRAdapter.Instance.SendSync(VideoTime);
	}

	void endOfVideo() {
		stop();
		_semispherePlayer.SetActive(false);
		_spherePlayer.SetActive(false);
		if(VRAdapter.Instance != null)
			VRAdapter.Instance.SendIsEmpty();
		onVideoEnds.Invoke();
		blackScreen.SetActive(false);
		BinauralAudio = false;
		ClearOptions();
	}

	public void DisplayChoice(string question, string optionsDescriptions, string optionsPositions) //TODO also needs positions
	{
		//display the last frame:
		//TODO must match the frame used for setting the position on the guide app
		VideoTime = player.Info.GetDurationFrames();
		pausePlayback();

		int i = 0;
		foreach (var optionDescription in optionsDescriptions.Split(','))
		{
			GameObject optionGameObject = Instantiate(optionMeshPrefab, optionsParent);
			optionGameObject.GetComponentInChildren<TextMesh>().text = optionDescription;
			optionGameObject.GetComponentInChildren<VRGazeChoice>().choiceIndex = i;

			var optionPosition = optionsPositions.Split('#')[i];
			float x = float.Parse(optionPosition.Split(',')[0]);
			float y = float.Parse(optionPosition.Split(',')[1]);
			float z = float.Parse(optionPosition.Split(',')[2]);
			optionGameObject.transform.eulerAngles = new Vector3(x, y, z);
			
			i++;
		}

		choiceContainer.SetActive(true);
	}
	
	public void OnSelectOption(int whichOption) {
		choiceContainer.SetActive(false);
		StopVideo();
		blackScreen.SetActive(true);//Set up the black screen until the follow-up video starts playing.
		VRAdapter.Instance.SendSelectOption((byte)whichOption);
		ClearOptions();
		Haze.Logger.Log("Selecting option " + whichOption);
		//TODO Should we display something while we wait for the next video to load and show up?... maybe.
	}

	public void OnEditOption(string videoName, string description, Vector3 eulerAngles)
	{
		_LoadingForChoicePosition = true;
	}
	public void Reorient(Vector3 eulerAngles) {
		rotator.localEulerAngles = eulerAngles;
	}

	//Received when the guide attempts to change the time on the video.
	public void Goto(double time) {
		VideoTime = time;
		PlaybackSpeed = 1;
		sendImmediateSync();
	}

	private void ClearOptions()
	{
		while (optionsParent.childCount > 0) DestroyImmediate(optionsParent.GetChild(0).gameObject);
	}
}
