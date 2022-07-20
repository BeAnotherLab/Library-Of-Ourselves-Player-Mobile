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
	[Header("Set to false to use the video shelf underneath each video")]
	[SerializeField] bool UseFullscreenVideoShelf = true;

	public string FullPath { get; private set; }//path to guide video
	public string VideoName { get; private set; }
	public VideoSettings Settings { get; set; }

	public static VideoDisplay expandedDisplay = null;

	bool __available = true;
	public bool Available {
		get { return __available; }
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
	
	public VideoDisplay Init(string path, string videoName, VideoSettings settings) {
		if(initialized) {
			Haze.Logger.LogError("Cannot initialize a VideoDisplay twice!!");
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
		thumbnailPath += videoName;
		//try several known extensions:
		Sprite thumbnail = PngToSprite.LoadSprite(thumbnailPath + ".png");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".PNG");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".jpg");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".JPG");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".jpeg");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".JPEG");
		if(thumbnail == null) {
			//none of the extensions have worked, keep the default in that case.
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
		} else {
			Haze.Logger.LogError("Error: no GuideVideoPlayer instance!");
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
		expandedDisplay = this;

		if(UseFullscreenVideoShelf) {
			Singleton videoShelf = Singleton.GetInstance("videoshelf");
			videoShelf.gameObject.SetActive(true);
			videoShelf.GetComponentInChildren<VideoShelf>().DisplayCurrentVideo();
			Haze.Logger.Log("Expanding.");

		} else {

			if(previouslyExpanded == this) {
				//we're already expanded; just update the display then.
				VideoShelf shelf = transform.parent.parent.GetComponent<VideoShelf>();
				if(shelf != null) shelf.DisplayCurrentVideo();
			} else {

				if(previouslyExpanded != null) {
					previouslyExpanded.contract();
				}

				VideoShelf shelf = transform.parent.parent.GetComponent<VideoShelf>();
				if(shelf != null) shelf.DisplayCurrentVideo();
				Interpolation expanding = transform.parent.parent.GetComponent<Interpolation>();
				if(expanding != null) expanding.Interpolate();

				StartCoroutine(scrollDown());
			}
		}

	}

	public void contract() {
		expandedDisplay = null;
		if(UseFullscreenVideoShelf) {
			//nothing to do here.
		} else {
			Interpolation expanding = transform.parent.parent.GetComponent<Interpolation>();
			expanding.InterpolateBackward();
		}
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
