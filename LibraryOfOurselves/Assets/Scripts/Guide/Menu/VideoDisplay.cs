using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class VideoDisplay : MonoBehaviour
{
	[SerializeField] Text videoNameDisplay;
	[SerializeField] Image videoThumbnail;
	[SerializeField] UnityEvent onBecomesAvailable;
	[SerializeField] UnityEvent onBecomesUnavailable;

	public string FullPath { get; private set; }//path to guide video
	public string VideoName { get; private set; } //TODO move file reference stuff to its own scriptable object instead of using UI scripts
	public VideoSettings Settings { get; set; }

	public static VideoDisplay expandedDisplay = null; //TODO use scriptable objects instead of singletons!

	bool __available = true;
	public bool Available { //TODO why are we setting a global state variable on a UI script?
		get { return __available; }
		set {
			if(__available != value) {
				__available = value;

				if (value) onBecomesAvailable.Invoke();
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
	
	public VideoDisplay Init(string path, string videoName, VideoSettings settings) { //TODO separate model and view 
		if (initialized) {
			Haze.Logger.LogError("Cannot initialize a VideoDisplay twice!!");
			return null;
		}

		FullPath =  //TODO remove this field, unnecessary;
		VideoName = videoName;
		Settings = settings;
		Available = false; 

		videoNameDisplay.text = VideoName;

		string[] split = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
		string thumbnailPath = "";
		for(int i = 0; i < split.Length - 1; ++i) thumbnailPath += split[i] + "/";
		thumbnailPath += videoName;

		Sprite thumbnail = PngToSprite.LoadSprite(thumbnailPath + ".png"); //TODO put in a foreach loop
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".PNG");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".jpg");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".JPG");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".jpeg");
		thumbnail = thumbnail ?? PngToSprite.LoadSprite(thumbnailPath + ".JPEG");
		
		//string[] x = {".png", ".PNG", ".jpg", ".JPG", ".jpeg", ".JPEG"};
		// foreach (string fileExtension in x) thumbnail = PngToSprite.LoadSprite(thumbnailPath + fileExtension);
		
		if (thumbnail != null) videoThumbnail.sprite = thumbnail;

		return this;
	}

	public void OnClickChoose() {
		//Load this video.
		GuideVideoPlayer player = GuideVideoPlayer.Instance;
		if (player != null) player.LoadVideo(this); //TODO is nullcheck necessary?
		else Haze.Logger.LogError("Error: no GuideVideoPlayer instance!"); //TODO remove?
	}

	public void OnClickSelectVideo() {
		StopAllCoroutines();
		if (expandedDisplay == this) Contract(); 
		else Expand();
	}

	public void Expand() {
		expandedDisplay = this;

		Singleton videoShelf = Singleton.GetInstance("videoshelf");
		videoShelf.gameObject.SetActive(true);
		videoShelf.GetComponentInChildren<VideoShelf>().DisplayCurrentVideo();
		Haze.Logger.Log("Expanding.");
	}

	public void Contract() {
		expandedDisplay = null;
	}

}
