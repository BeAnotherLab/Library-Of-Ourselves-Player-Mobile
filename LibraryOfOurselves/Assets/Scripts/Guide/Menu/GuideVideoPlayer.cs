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

	public static GuideVideoPlayer Instance { get; private set; }

	double VideoTime { get { return videoPlayer.time; } }

	public bool Playing { get; private set; }

	float TotalVideoTime { get { return (float)(videoPlayer.frameCount / videoPlayer.frameRate); } }

	bool displaying = false;//switched to true as soon as we're displaying something.
	bool allDevicesReady = false;
	bool startedPlayback = false;//switched to true when we press Play the first time.

	float lastTimeShown = 0;

	private void Start() {
		Instance = this;
		onStop.Invoke();

		timeSlider.onValueChanged.AddListener(delegate (float val) {
			float time = val * TotalVideoTime;
			videoPlayer.time = time;
		});

		videoPlayer.loopPointReached += delegate (VideoPlayer player) {
			Stop();
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
	}

	async void SendSyncMessages() {
		while(Playing && GuideAdapter.Instance) {
			GuideAdapter.Instance.SendSync(VideoTime);
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
				}
			}

			//show time on slider
			if(!Mathf.Approximately((float)videoPlayer.time, lastTimeShown) && videoPlayer.isPlaying) {
				lastTimeShown = (float)videoPlayer.time;
				timeSlider.SetValueWithoutNotify((float)videoPlayer.time / TotalVideoTime);
			}

			//if no one is paired anymore, just stop.
			if(TCPHost.Instance != null && TCPHost.Instance.NumberOfPairedDevices <= 0) {
				Stop();
			}

		}
	}

}
