using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class GuideVideoPlayer : MonoBehaviour{

	[SerializeField] float timeBetweenSyncs = 0.75f;

	public static GuideVideoPlayer Instance { get; private set; }

	double VideoTime { get { return 1; } }//TODO

	public bool Playing { get; private set; }

	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
		Playing = false;
	}

	public void ___HasDummy___() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendHasVideo("DalvaEN");//("Atelier1");
	}

	public void ___LoadDummy___() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendLoadVideo("DalvaEN", "235");//("Atelier1", "360");
	}

	public void Play() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendPlayVideo();
		Playing = true;
		SendSyncMessages();
	}

	public void Pause() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendPauseVideo(VideoTime);
		Playing = false;
	}

	public void Stop() {
		if(GuideAdapter.Instance)
			GuideAdapter.Instance.SendStopVideo();
		Playing = false;
	}

	async void SendSyncMessages() {
		while(Playing && GuideAdapter.Instance) {
			GuideAdapter.Instance.SendSync(VideoTime);
			await Task.Delay((int)(timeBetweenSyncs * 1000));
		}
	}

}
