using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.Networking;

public class VRVideoPlayer : MonoBehaviour{

	[SerializeField] float timeBetweenSyncs = 0.75f;
	[SerializeField] GameObject semispherePlayer;
	[SerializeField] GameObject spherePlayer;
	[SerializeField] AudioSource leftAudio;
	[SerializeField] AudioSource rightAudio;
	[SerializeField] UnityEvent onVideoEnds;
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onPlay;
	[SerializeField] OVRInput.Button oculusGoRecenterButton = OVRInput.Button.PrimaryIndexTrigger;//change this to change which button controls recenter on oculus go
	[SerializeField] GvrControllerButton mirageRecenterButton = GvrControllerButton.TouchPadButton;//change this to change which button controls recenter on mirage solo
	[SerializeField] OVRInput.Button oculusGoQuitButton = OVRInput.Button.PrimaryThumbstick;
	[SerializeField] GvrControllerButton mirageQuitButton = GvrControllerButton.App;
	[SerializeField] GameObject choiceContainer;
	[SerializeField] TextMesh questionMesh;
	[SerializeField] TextMesh option1Mesh;
	[SerializeField] TextMesh option2Mesh;
	[SerializeField] GameObject blackScreen;
	[SerializeField] Transform rotator;
	[SerializeField] AudioLoadingMode audioLoadingMode = AudioLoadingMode.UnityWebResource;

	[Header("Sync settings")]
	[SerializeField] float allowedErrorForSyncedPlayback = 0.5f;
	[SerializeField] float maximumAllowedErrorBeforeResync = 3.0f;
	[SerializeField] float maximumPlaybackSpeed = 1.5f;
	[SerializeField] float minimumPlaybackSpeed = 0.75f;

	public enum AudioLoadingMode {
		WWW,
		NLayer,
		UnityWebResource
	}


	public static VRVideoPlayer Instance { get; private set; }

	VideoPlayer player;
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
			player.audioOutputMode = value ? VideoAudioOutputMode.None : VideoAudioOutputMode.Direct;
		}
	}

	double VideoTime {
		get { return player.time; }
		set {
			player.time = value;
			if(BinauralAudio) {
				leftAudio.time = (float)value;
				rightAudio.time = (float)value;
			}
		}
	}

	float PlaybackSpeed {
		set {
			player.playbackSpeed = value;
			if(BinauralAudio) {
				leftAudio.pitch = value;
				rightAudio.pitch = value;
			}
		}
	}

	public struct VideoLoadingResponse { public bool ok;public string errorMessage; }

	long lastReadyFrame = -1;

	private void Start() {
		Camera.main.backgroundColor = DeviceColour.getDeviceColor(SystemInfo.deviceUniqueIdentifier);//Set background colour to something unique
		Instance = this;
		mainCamera = Camera.main;
		player = GetComponent<VideoPlayer>();

		choiceContainer.SetActive(false);
		blackScreen.SetActive(false);

		BinauralAudio = false;

		player.errorReceived += delegate (VideoPlayer player, string message) {
			Debug.LogError("VideoPlayer error: " + message);
			errorWhileLoading = true;
			endOfVideo();
		};
		player.loopPointReached += delegate (VideoPlayer player) {
			//as long as we havent received the Stop message, we can simply keep on playing.
			play();
		};
		player.frameReady += delegate (VideoPlayer unused, long frame) {
			lastReadyFrame = frame;
		};
	}

	private void OnDestroy() {
		Instance = null;
	}

	static string getPath(string videoName) {
		return Filesystem.SDCardRoot + videoName + ".mp4";
	}

	static string getAudioPath(string videoName, bool left) {
		return Filesystem.SDCardRoot + videoName + "-" + (left ? "l" : "r"); //extension will be assumed .wav unless the wave file doesn't exist in which case .mp3 will be selected
	}

	public static bool IsVideoAvailable(string videoName) {

		string path = getPath(videoName);
		Debug.Log("Checking if we have " + path);
		if(File.Exists(path)) {
			return true;
		}

		return false;
	}

	public async Task<VideoLoadingResponse> LoadVideo(string videoName, string mode) {
		VideoLoadingResponse response = new VideoLoadingResponse();
		response.ok = true;
		response.errorMessage = "";

		Debug.Log("Loading video " + videoName + "...");
		errorWhileLoading = false;

		if(!IsVideoAvailable(videoName)) {
			response.errorMessage = "Video unavailable.";
			response.ok = false;
			Debug.LogError("Video " + videoName + " could not be found.");
			return response;
		}

		is360 = false;
		if(mode.Length >= 3 && mode[0] == '3' && mode[1] == '6' && mode[2] == '0') {
			//360 video.
			is360 = true;
			Debug.Log("Loading 360 degree video.");
		} else Debug.Log("Loading 235 degree video.");

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

		if(File.Exists(leftAudioFile) && File.Exists(rightAudioFile)) {
			BinauralAudio = true;

			//load the sound into the audio sources
			switch(audioLoadingMode) {
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
		} else {
			BinauralAudio = false;
		}
		Debug.Log("Binaural audio: " + (BinauralAudio ? "on" : "off"));
		if(BinauralAudio) {
			Debug.Log("Loading binaural audio files: " + leftAudioFile + " and " + rightAudioFile);
		}
		
		//Prepare video player
		player.url = getPath(videoName);
		PlaybackSpeed = 1;
		lastReadyFrame = -1;
		player.sendFrameReadyEvents = true;//will only send up until the end of this method where we re-disable this param
		player.Prepare();
		Debug.Log("Preparing player: CanSetTime=" + player.canSetTime + ", CanSetPlaybackSpeed=" + player.canSetPlaybackSpeed);

		//Load the video into the player...
		DateTime before = DateTime.Now;
		while(!player.isPrepared && !errorWhileLoading) {
			await Task.Delay(20);
		}
		if(errorWhileLoading) {
			response.ok = false;
			response.errorMessage = "Could not load video...";
			return response;
		}
		TimeSpan took = DateTime.Now - before;
		Debug.Log("Player is prepared! Took: " + took.TotalMilliseconds + " ms.");

		//Wait for the audio sources to load
		if(BinauralAudio) {
			Debug.Log("Waiting for audio files to load...");

			switch(audioLoadingMode) {
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
						Debug.LogError("Could not open audio file " + leftAudioFile + ": " + leftuwr.error);
					if(rightuwr.isNetworkError)
						Debug.LogError("Could not open audio file " + rightAudioFile + ": " + rightuwr.error);
					break;
			}

			Debug.Log("Audio loaded: Left: " + leftAudio.clip.loadState + " / Right: " + rightAudio.clip.loadState);

		}

		Reorient(Vector3.zero);//reset orientation

		//Show first frame as soon as it's loaded in and rendered, making sure the video is 100% paused when we do so.
		float previousVolume = Volume;
		Volume = 0;//mute it for now
		player.Play();//we do need to start playing it to render the frame, but we'll be pausing immediately after
		while(lastReadyFrame == -1) {
			//no frames have been readied so far...
			await Task.Delay(20);
		}//at least one frame has been marked as ready, so now we should be able to display the video
		player.sendFrameReadyEvents = false;//stop sending them, we only need the callback for the initial frame
		player.time = 0;
		await Task.Delay(10);
		player.time = 0;
		player.Pause();//make sure the video is paused for now...
		player.time = 0;
		Volume = previousVolume;//restore volume
		await Task.Delay(100);//wait a tenth of a second just to be sure the first frame has been rendered to the render texture
		//and display on screen:
		blackScreen.SetActive(false);
		if(is360) spherePlayer.SetActive(true);
		else semispherePlayer.SetActive(true);

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
			else if(player.audioTrackCount > 0)
				return player.GetDirectAudioVolume(0);
			else return 0;
		}
		set {
			if(BinauralAudio) {
				leftAudio.volume = value;
				rightAudio.volume = value;
			} else {
				for(ushort track = 0; track < player.audioTrackCount; ++track) {
					player.SetDirectAudioVolume(track, value);
				}
			}
		}
	}

	public void PlayVideo(DateTime timestamp) {

		blackScreen.SetActive(false);

		//figure out difference in time between now and timestamp
		TimeSpan difference = DateTime.Now - timestamp;
		Debug.Log("Started playing video " + difference.TotalSeconds + " s ago.");

		VideoTime = 0;
		play();
		
		if(is360) spherePlayer.SetActive(true);
		else semispherePlayer.SetActive(true);

		onPlay.Invoke();

		SendSyncMessages();
		sendImmediateSync();
	}

	public void Sync(DateTime unused, double videoTime) {
		//Assume at timestamp it was at videoTime; if it would've been later, slow down time slightly; if it would've been earlier, speed up time slightly
		float targetTime = (float)videoTime;
		float actualTime = (float)player.time;
		float delta = actualTime - targetTime;//Negative->go faster; Positive->go slower

		//Shall we speed up or slow down?
		if(Mathf.Abs(delta) < allowedErrorForSyncedPlayback) {
			PlaybackSpeed = 1;
		}else if(Mathf.Abs(delta) > maximumAllowedErrorBeforeResync) {//too much difference, let's just pop back to the right point
			Debug.Log("Target time = " + targetTime + " / Actual time = " + actualTime + " // Difference = " + delta + " ==> Too much difference, jumping to " + targetTime);
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

		if(!player.isPlaying) {
			Debug.LogWarning("Player was stopped when we received Sync message");
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
			if(player.isPlaying) {
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
		semispherePlayer.SetActive(false);
		spherePlayer.SetActive(false);
		if(VRAdapter.Instance != null)
			VRAdapter.Instance.SendIsEmpty();
		onVideoEnds.Invoke();
		blackScreen.SetActive(false);
		BinauralAudio = false;
	}


	void Update() {
		if(VRDevice.OculusGo) {//On OculusGo, use the controller's trigger to recalibrate
			OVRInput.Update();
			if(OVRInput.Get(oculusGoRecenterButton)) {
				Recenter();
			}
			if(OVRInput.Get(oculusGoQuitButton)) {
				Application.Quit();
			}
		} else if(VRDevice.MirageSolo) {//On Mirage Solo, use the controller's click button
			bool overrideTouch = false;
			bool overrideQuit = false;
			GvrControllerInputDevice device = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);
			if(device == null) {
				device = GvrControllerInput.GetDevice(GvrControllerHand.NonDominant);
				if(device == null) {
					overrideTouch = GvrControllerInput.ClickButton;
					overrideQuit = GvrControllerInput.AppButton;
				}
			}
			if((device != null && device.GetButton(mirageRecenterButton)) || overrideTouch) {
				Recenter();
			}
			if((device != null && device.GetButton(mirageQuitButton)) || overrideQuit) {
				Application.Quit();
			}
		} else if(VRDevice.GearVR) {//On GearVR, double-tap to recalibrate
			if(firstTap > 0) {//we've tapped a first time!
				if(Input.GetMouseButtonDown(0)) {
					//that's it, double-tapped.
					Recenter();
					firstTap = 0;
				}
				firstTap -= Time.deltaTime;
			} else if(Input.GetMouseButtonUp(0)) {
				firstTap = 0.3f;//you have .3 seconds to tap once more for double tap!
			}
		}
	}

	void FixedUpdate() {
		if(VRDevice.OculusGo)
			OVRInput.FixedUpdate();
	}

	public void DisplayChoice(string question, string choice1, string choice2) {
		//display the last frame:
		VideoTime = player.length;
		pausePlayback();

		questionMesh.text = question;
		option1Mesh.text = choice1;
		option2Mesh.text = choice2;

		choiceContainer.SetActive(true);
	}

	public void OnSelectOption(int whichOption) {
		choiceContainer.SetActive(false);
		StopVideo();
		blackScreen.SetActive(true);//Set up the black screen until the follow-up video starts playing.
		VRAdapter.Instance.SendSelectOption((byte)whichOption);
		Debug.Log("Selecting option " + whichOption);
		//Should we display something while we wait for the next video to load and show up?... maybe.
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

}
