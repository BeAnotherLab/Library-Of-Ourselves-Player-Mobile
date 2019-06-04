using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;

public class GuideVideoPlayer : MonoBehaviour{

	[SerializeField] float timeBetweenSyncs = 0.75f;
	[SerializeField] Slider timeSlider;
	[SerializeField] VideoPlayer videoPlayer;
	[SerializeField] UnityEvent onLoad;
	[SerializeField] UnityEvent onStop;
	[SerializeField] UnityEvent onAllDevicesReady;
	[SerializeField] UnityEvent onFirstFrameReady;
	[SerializeField] UnityEvent onPlay;
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onNextVideoIsUnavailable;

	public static GuideVideoPlayer Instance { get; private set; }

	double VideoTime { get { return videoPlayer.time; } }

	public bool Playing { get; private set; }

	public bool HasVideoLoaded { get; private set; }

	float TotalVideoTime { get { return (float)(videoPlayer.frameCount / videoPlayer.frameRate); } }

	public VideoDisplay CurrentVideo { get { return currentVideo; } }


	bool displaying = false;//switched to true as soon as we're displaying something.
	bool allDevicesReady = false;
	bool startedPlayback = false;//switched to true when we press Play the first time.

	float lastTimeShown = 0;
	bool playImmediately = false;//when true, will play the next video loaded immediately once it's loaded.

	VideoDisplay currentVideo = null;

	private void Start() {
		Instance = this;
		onStop.Invoke();

		HasVideoLoaded = false;

		timeSlider.onValueChanged.AddListener(delegate (float val) {
			float time = val * TotalVideoTime;
			videoPlayer.time = time;
		});

		videoPlayer.loopPointReached += delegate (VideoPlayer player) {
			//Display a choice?
			if(currentVideo != null && currentVideo.Settings.choices.Length > 0) {
				VideosDisplayer.VideoChoice choice = currentVideo.Settings.choices[0];
				Debug.Log("Displaying choice [" + choice.question + "]: " + choice.option1 + " / " + choice.option2);
				GuideAdapter.Instance.SendStartChoice(choice.question, choice.option1, choice.option2);
				Playing = false;
			} else {
				Stop();
			}
		};

		videoPlayer.sendFrameReadyEvents = true;
		videoPlayer.frameReady += delegate (VideoPlayer player, long frame) {
			if(frame == 1) {
				lastTimeShown = (float)player.time;
				player.time = timeSlider.value * TotalVideoTime;
				onFirstFrameReady.Invoke();
			}
		};
	}

	private void OnDestroy() {
		Instance = null;
		Playing = false;
	}

	public void LoadVideo(VideoDisplay videoDisplay) {

		currentVideo = videoDisplay;

		if(GuideAdapter.Instance) {
			GuideAdapter.Instance.SendLoadVideo(videoDisplay.VideoName, videoDisplay.Settings.is360 ? "360" : "235");
			displaying = true;
			allDevicesReady = false;
			startedPlayback = false;
			lastTimeShown = 0;

			videoPlayer.url = videoDisplay.FullPath;

			timeSlider.SetValueWithoutNotify(0);

			onLoad.Invoke();
			onPause.Invoke();

			HasVideoLoaded = true;
			ConnectionsDisplayer.UpdateAllDisplays();
		}
	}

	public void Play() {//the correct command is Play only when we start playback, afterwards it's Pause (which toggles between the two)
		if(startedPlayback) {
			Pause();
			return;
		}

		onPlay.Invoke();

		videoPlayer.Play();

		startedPlayback = true;
		if(GuideAdapter.Instance) {
			GuideAdapter.Instance.SendPlayVideo();
			GuideAdapter.Instance.SendSync(videoPlayer.time);
		}
		Playing = true;
		SendSyncMessages();
	}

	void Pause() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendPauseVideo(VideoTime);
		Playing = !Playing;
		if(Playing) {
			videoPlayer.Play();
			onPlay.Invoke();
		} else {
			videoPlayer.Pause();
			onPause.Invoke();
		}
	}

	public void Stop() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendStopVideo();
		Playing = false;
		displaying = false;
		onStop.Invoke();

		HasVideoLoaded = false;

		currentVideo = null;
	}

	async void SendSyncMessages() {
		while(Playing && GuideAdapter.Instance) {
			if(Settings.SendSyncMessages) {//only if the behaviour is allowed by the current settings.
				GuideAdapter.Instance.SendSync(VideoTime);
			}
			await Task.Delay((int)(timeBetweenSyncs * 1000));
		}
	}

	private void Update() {
		if(displaying) {
			if(!allDevicesReady) {
				//Waiting for the devices to become ready...
				allDevicesReady = true;
				if(ConnectionsDisplayer.Instance) {
					foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
						if(handle.connection.paired && !handle.display.IsVideoReady) {
							//this one's not ready yet...
							allDevicesReady = false;
							break;
						}
					}
				}

				//if it's still true it means they're all ready now :)
				if(allDevicesReady) {
					onAllDevicesReady.Invoke();
					if(playImmediately) {
						Play();
						playImmediately = false;
					}
				}
			}

			//show time on slider
			if(!Mathf.Approximately((float)videoPlayer.time, lastTimeShown) && videoPlayer.isPlaying) {
				lastTimeShown = (float)videoPlayer.time;
				timeSlider.SetValueWithoutNotify((float)videoPlayer.time / TotalVideoTime);
			}

			//check whether it's time to send a reorient?
			sendAnyRequiredReorient();

			//if no one is paired anymore, just stop.
			if(TCPHost.Instance != null && TCPHost.Instance.NumberOfPairedDevices <= 0) {
				Stop();
			}

		}
	}

	public void OnReceiveChoiceConfirmation(TCPConnection connection, int choiceIndex) {
		//choice index is either 0 or 1
		if(currentVideo != null && currentVideo.Settings.choices.Length > 0) {
			string nextVideo = choiceIndex == 0 ? currentVideo.Settings.choices[0].video1 : currentVideo.Settings.choices[0].video2;
			VideoDisplay nextVideoDisplay = VideosDisplayer.Instance.FindVideo(nextVideo);
			if(nextVideoDisplay != null && nextVideoDisplay.Available) {
				playImmediately = true;
				LoadVideo(nextVideoDisplay);
			} else {
				Debug.LogError("Doesn't have video: " + nextVideo);
				onNextVideoIsUnavailable.Invoke();
				Stop();
			}
		}
	}

	Vector4 deltaAnglesPreviouslySent = Vector4.zero;
	void sendAnyRequiredReorient() {
		if(currentVideo == null) return;
		//check which reorient is the closest to the current video player time (without overstepping at all)
		float currentTime = (float)VideoTime;
		float closestTime = -1;
		Vector4 closestDeltaAngles = Vector4.zero;
		foreach(Vector4 deltaAngles in currentVideo.Settings.deltaAngles) {
			if(deltaAngles.w <= currentTime && deltaAngles.w > closestTime) {
				closestDeltaAngles = deltaAngles;
			}
		}
		//did we find one
		if(closestTime >= 0) {
			//have we already sent that one?
			if(deltaAnglesPreviouslySent != closestDeltaAngles) {
				//nope, alright lets send it then
				deltaAnglesPreviouslySent = closestDeltaAngles;
				if(GuideAdapter.Instance != null) {
					//and fire it off!
					GuideAdapter.Instance.SendReorient(new Vector3(closestDeltaAngles.x, closestDeltaAngles.y, closestDeltaAngles.z));
				}
			}
		}
	}

}
