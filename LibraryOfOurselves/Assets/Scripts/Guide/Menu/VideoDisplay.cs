using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class VideoDisplay : MonoBehaviour{

	[SerializeField] Text videoNameDisplay;
	[SerializeField] Image videoThumbnail;
	[SerializeField] UnityEvent onBecomesAvailable;
	[SerializeField] UnityEvent onBecomesUnavailable;

	public string FullPath { get; private set; }//path to guide video
	public string VideoName { get; private set; }
	public VideosDisplayer.VideoSettings Settings { get; set; }

	public static VideoDisplay expandedDisplay = null;

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

	public Sprite Thumbnail {
		get {
			return videoThumbnail.sprite;
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

		string[] split = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
		string thumbnailPath = "";
		for(int i = 0; i < split.Length - 1; ++i) thumbnailPath += split[i] + "/";
		thumbnailPath += videoName + ".png";
		Sprite thumbnail = PngToSprite.LoadSprite(thumbnailPath);
		if(thumbnail == null) {
			//No thumbnail for this one i guess, keep the default.
		} else {
			videoThumbnail.sprite = thumbnail;
		}

		return this;
	}

	public void OnClickChoose() {
		//Load this video.
		GuideVideoPlayer player = GuideVideoPlayer.Instance;
		if(player != null) {
			player.LoadVideo(this);
		}
	}

	public void OnClickSelectVideo() {
		//if we're already expanded, contract. otherwise expand.
		StopAllCoroutines();
		if(expandedDisplay == this) {
			contract();
		} else {
			expand();
		}
	}

	//Display this video's settings and such.
	public void expand() {
		VideoDisplay previouslyExpanded = expandedDisplay;
		if(previouslyExpanded != null) {
			previouslyExpanded.contract();
		}
		expandedDisplay = this;

		VideoShelf shelf = transform.parent.parent.GetComponent<VideoShelf>();
		if(shelf != null) shelf.DisplayCurrentVideo();
		Interpolation expanding = transform.parent.parent.GetComponent<Interpolation>();
		if(expanding != null) expanding.Interpolate();

		StartCoroutine(scrollDown());

	}

	public void contract() {
		expandedDisplay = null;
		Interpolation expanding = transform.parent.parent.GetComponent<Interpolation>();
		expanding.InterpolateBackward();
	}

	IEnumerator scrollDown() {
		ScrollRect scrollRect = GetComponentInParent<ScrollRect>();

		float normalScreenY = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position).y / Screen.height;
		while(normalScreenY < 0.6f) {
			normalScreenY = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position).y / Screen.height;
			//scroll down a bit
			float scrollY = scrollRect.verticalNormalizedPosition * scrollRect.content.rect.height;
			scrollY -= Time.deltaTime * 2000;
			scrollRect.verticalNormalizedPosition = scrollY / scrollRect.content.rect.height;
			yield return null;
		}
	}

}
