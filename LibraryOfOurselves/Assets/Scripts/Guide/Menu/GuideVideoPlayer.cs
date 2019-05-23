using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Video;

public class GuideVideoPlayer : MonoBehaviour{

	[SerializeField] float timeBetweenSyncs = 0.75f;
	[SerializeField] VideoPlayer videoPlayer;
	[SerializeField] UnityEvent onLoad;
	[SerializeField] UnityEvent onStop;
	[SerializeField] UnityEvent onAllDevicesReady;

	public static GuideVideoPlayer Instance { get; private set; }

	double VideoTime { get { return videoPlayer.time; } }

	public bool Playing { get; private set; }

	bool displaying = false;//switched to true as soon as we're displaying something.
	bool allDevicesReady = false;
	bool startedPlayback = false;//switched to true when we press Play the first time.

	private void Start() {
		Instance = this;
		onStop.Invoke();

		videoPlayer.loopPointReached += delegate (VideoPlayer player) {
			Stop();
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

			videoPlayer.url = videoDisplay.FullPath;

			onLoad.Invoke();
		}
	}

	public void Play() {//the correct command is Play only when we start playback, afterwards it's Pause (which toggles between the two)
		if(startedPlayback) {
			Pause();
			return;
		}

		startedPlayback = true;
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendPlayVideo();
		Playing = true;
		SendSyncMessages();

		videoPlayer.Play();
	}

	void Pause() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendPauseVideo(VideoTime);
		Playing = !Playing;
		if(Playing) {
			videoPlayer.Play();
		} else {
			videoPlayer.Pause();
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

			//if no one is paired anymore, just stop.
			if(TCPHost.Instance != null && TCPHost.Instance.NumberOfPairedDevices <= 0) {
				Stop();
			}

		}
	}

}
