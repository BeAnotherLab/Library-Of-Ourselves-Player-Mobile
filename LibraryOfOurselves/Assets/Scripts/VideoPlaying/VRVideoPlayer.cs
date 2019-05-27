using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.Video;
using UnityEngine.Events;

public class VRVideoPlayer : MonoBehaviour{

	[SerializeField] GameObject semispherePlayer;
	[SerializeField] GameObject spherePlayer;
	[SerializeField] UnityEvent onVideoEnds;
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onPlay;
	[SerializeField] OVRInput.Button oculusGoRecenterButton = OVRInput.Button.PrimaryIndexTrigger;//change this to change which button controls recenter on oculus go
	[SerializeField] GvrControllerButton mirageRecenterButton = GvrControllerButton.TouchPadButton;//change this to change which button controls recenter on mirage solo

	public static VRVideoPlayer Instance { get; private set; }

	VideoPlayer player;
	Camera mainCamera;
	bool errorWhileLoading = false;
	float firstTap = 0;
	bool is360;//whether the current video is 360 degrees

	float timeDifference = 0;//How much difference there is between the guide and the user app when we start playing the video.

	public struct VideoLoadingResponse { public bool ok;public string errorMessage; }


	private void Start() {
		Camera.main.backgroundColor = DeviceColour.getDeviceColor(SystemInfo.deviceUniqueIdentifier);//Set background colour to something unique
		Instance = this;
		mainCamera = Camera.main;
		player = GetComponent<VideoPlayer>();

		player.errorReceived += delegate (VideoPlayer player, string message) {
			Debug.LogError("VideoPlayer error: " + message);
			errorWhileLoading = true;
			endOfVideo();
		};
		player.loopPointReached += delegate (VideoPlayer player) {
			//as long as we havent received the Stop message, we can simply keep on playing.
			player.Play();
		};
	}

	private void OnDestroy() {
		Instance = null;
	}

	static string getPath(string videoName) {
		return Filesystem.SDCardRoot + videoName + ".mp4";
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
			return response;
		}

		is360 = false;
		if(mode.Length >= 3 && mode[0] == '3' && mode[1] == '6' && mode[2] == '0') {
			//360 video.
			is360 = true;
		}
		
		player.url = getPath(videoName);
		player.playbackSpeed = 1;
		player.Prepare();
		Debug.Log("Preparing player: CanSetTime=" + player.canSetTime + ", CanSetPlaybackSpeed=" + player.canSetPlaybackSpeed);

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

		return response;
	}

	public void PlayVideo(DateTime timestamp) {
		
		//figure out difference in time between now and timestamp
		TimeSpan difference = DateTime.Now - timestamp;
		Debug.Log("Started playing video " + difference.TotalSeconds + " s ago.");

		timeDifference = (float)difference.TotalSeconds;
		if(timeDifference > 0) {
			player.time = timeDifference;
			timeDifference = 0;
		}
		
		player.Play();
		
		if(is360) spherePlayer.SetActive(true);
		else semispherePlayer.SetActive(true);

		onPlay.Invoke();
	}

	public void Sync(DateTime timestamp, double videoTime) {
		//Assume at timestamp it was at videoTime; if it would've been later, slow down time slightly; if it would've been earlier, speed up time slightly
		TimeSpan difference = DateTime.Now - timestamp;
		float secondsDiff = (float)difference.TotalSeconds - timeDifference;
		float targetTime = (float)videoTime + secondsDiff;
		float actualTime = (float)player.time;
		float delta = actualTime - targetTime;//Negative->go faster; Positive->go slower

		//Shall we speed up or slow down?
		if(Mathf.Abs(delta) < 0.5f) {
			player.playbackSpeed = 1;
		}else if(Mathf.Abs(delta) > 3) {//too much difference, let's just pop back to the right point
			player.time = targetTime;
			player.playbackSpeed = 1;
		}else if(delta < 0) {// actualTime < targetTime -> go faster
			delta = Mathf.Abs(delta) - 0.5f;//0 when the difference is 0.5, 1 at 1.5 and higher
			if(delta > 1) delta = 1;
			player.playbackSpeed = 1 + delta * 0.3f;
		} else {// actualTime > targetTime -> go slower
			delta = delta - 0.5f;//0 when difference is 0.5, 1 at 1.5 and higher
			if(delta > 1) delta = 1;
			player.playbackSpeed = 1 - delta * 0.3f;
		}

		if(!player.isPlaying) {
			Debug.LogWarning("Player was stopped when we received Sync message");
			player.Play();
		}
	}

	//Toggles between playing and paused.
	public void PauseVideo(double videoTime) {
		if(player.isPaused) {
			player.time = videoTime;
			player.playbackSpeed = 1;
			player.Play();
			onPlay.Invoke();
		} else {
			player.Pause();
			player.time = videoTime;
			onPause.Invoke();
		}
	}

	public void StopVideo() {
		endOfVideo();
	}

	public void Recenter() {
		if(mainCamera == null) mainCamera = Camera.main;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
	}

	void endOfVideo() {
		player.Stop();
		semispherePlayer.SetActive(false);
		spherePlayer.SetActive(false);
		if(VRAdapter.Instance != null)
			VRAdapter.Instance.SendIsEmpty();
		onVideoEnds.Invoke();
	}


	void Update() {
		if(VRDevice.OculusGo) {//On OculusGo, use the controller's trigger to recalibrate
			OVRInput.Update();
			if(OVRInput.Get(oculusGoRecenterButton)) {
				Recenter();
			}
		} else if(VRDevice.MirageSolo) {//On Mirage Solo, use the controller's click button
			bool overrideTouch = false;
			GvrControllerInputDevice device = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);
			if(device == null) {
				device = GvrControllerInput.GetDevice(GvrControllerHand.NonDominant);
				if(device == null) {
					overrideTouch = GvrControllerInput.ClickButton;
				}
			}
			if((device != null && device.GetButton(mirageRecenterButton)) || overrideTouch) {
				Recenter();
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

}
