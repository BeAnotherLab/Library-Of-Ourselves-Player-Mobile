using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading.Tasks;

public class VRVideoPlayer : MonoBehaviour{

	public static VRVideoPlayer Instance { get; private set; }

	public bool IsVideoPlaying { get { return true; } }//TODO
	public bool IsVideoLoaded { get { return true; } }//TODO


	public struct VideoLoadingResponse { public bool ok;public string errorMessage; }


	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}


	public static bool IsVideoAvailable(string videoName) {

		Debug.Log("Checks if the video " + videoName + " is available");
		return true;//TODO
	}

	public async Task<VideoLoadingResponse> LoadVideo(string videoName) {
		VideoLoadingResponse response = new VideoLoadingResponse();
		response.ok = true;
		response.errorMessage = "";

		Debug.Log("Loads video " + videoName + "...");
		await Task.Delay(1000);
		Debug.Log("Dummy loaded!");

		return response;//TODO; only return once video is ready!
	}

	public void PlayVideo(DateTime timestamp) {
		//TODO: start playing video taking timestamp as the date when it was at 0
		Debug.Log("Starts playing at: " + timestamp.Hour + " : " + timestamp.Minute + " : " + timestamp.Second);
	}

	public void Sync(DateTime timestamp, double videoTime) {
		//TODO: assume at timestamp it was at videoTime; if it would've been later, slow down time slightly; if it would've been earlier, speed up time slightly
	}

	public void PauseVideo(double videoTime) {
		//TODO: pause video playback, and reset videoTime to the specified time
		Debug.Log("Pauses at: " + videoTime);
	}

	public async Task StopVideo() {
		Debug.Log("Stops video");
		//TODO: only return once we're ready to start loading a new video!
	}

	public void Recenter() {
		//TODO: recenter current player to the camera on Y rotation
		Debug.Log("Recenters camera");
	}

}
