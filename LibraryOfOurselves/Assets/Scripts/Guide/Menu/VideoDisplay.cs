using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VideoDisplay : MonoBehaviour{

	[SerializeField] Text videoNameDisplay;
	[SerializeField] Text descriptionDisplay;
	[SerializeField] Text objectsDisplay;
	[SerializeField] UnityEvent onBecomesAvailable;
	[SerializeField] UnityEvent onBecomesUnavailable;

	public string FullPath { get; private set; }//path to guide video
	public string VideoName { get; private set; }
	public VideosDisplayer.VideoSettings Settings { get; set; }

	bool __available = true;
	public bool Available {
		get { return Available; }
		set {
			if(__available != value) {
				__available = value;

				if(value) onBecomesAvailable.Invoke();
				else onBecomesUnavailable.Invoke();
			}
		}
	}

	bool initialized = false;
	public VideoDisplay Init(string path, string videoName, VideosDisplayer.VideoSettings settings) {
		if(initialized) {
			Debug.LogError("Cannot initialize a VideoDisplay twice!!");
			return null;
		}

		FullPath = path;
		VideoName = videoName;
		Settings = settings;
		Available = false;

		videoNameDisplay.text = VideoName;
		if(settings.description.Length > 0)
			descriptionDisplay.text = settings.description;
		if(settings.objectsNeeded.Length > 0)
			objectsDisplay.text = settings.objectsNeeded;

		return this;
	}

	public void OnClickChoose() {
		//Load this video.
		GuideVideoPlayer player = GuideVideoPlayer.Instance;
		if(player != null) {
			player.LoadVideo(this);
		}
	}

}
