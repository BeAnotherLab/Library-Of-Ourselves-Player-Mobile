using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.IO;
using System.Linq;
using SimpleFileBrowser;

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
	
	public VideoDisplay Init(string path, string videoName, VideoSettings settings) { //TODO separate model and view. Remove unused path param 
		if (initialized) {
			Haze.Logger.LogError("Cannot initialize a VideoDisplay twice!!");
			return null;
		}

		FullPath =  //TODO remove this field, unnecessary;
		VideoName = videoName;
		Settings = settings;
		Available = false; 

		videoNameDisplay.text = VideoName;
		
		var thumbnailFileSystemEntry = GetThumbnailPath(videoName);
		var tempFolder = Application.temporaryCachePath;
		
		if (thumbnailFileSystemEntry.Name != null)
		{
			var filePath = Path.Combine(tempFolder, thumbnailFileSystemEntry.Name);
			Debug.Log("copying file " + thumbnailFileSystemEntry.Path + " to " + filePath);
			FileBrowserHelpers.CopyFile(thumbnailFileSystemEntry.Path, filePath);

			Debug.Log("looking for thumbnail after copying at " + filePath);
		
			if (File.Exists(filePath))
			{
				if (thumbnailFileSystemEntry.Path != "")
				{
					Texture2D texture = NativeGallery.LoadImageAtPath(filePath, markTextureNonReadable: false); //TODO FileNotFound!!
			
					if( texture == null ) Debug.Log( "Couldn't load texture from " + filePath );
            
					Sprite thumbnail = Sprite.Create(texture,
						new Rect(0, 0, texture.width, texture.height),
						new Vector2(texture.width/2, texture.height/2));
				
					if (thumbnail != null) videoThumbnail.sprite = thumbnail;
				}
			}
			else
			{
				Debug.Log("couldn't find file at : " + filePath);
			}
		}
				
		return this;
	}

	private FileSystemEntry GetThumbnailPath(string videoName)
	{
		//TODO check if case senstiive. see how to do case insensitive comparison
		var extensions = new [] { ".PNG", ".png", ".jpg", ".JPG", ".jpeg", ".JPEG", ".Jpeg" };

		foreach (FileSystemEntry file in FileBrowserHelpers.GetEntriesInDirectory(DataFolder.GuidePath, true))
		{
			//TODO this will cause problem if same string is found in different videos?
			Debug.Log("checking filesystem entry " + file.Path);
			if (file.Name.IndexOf(videoName, 0, StringComparison.Ordinal) != -1) //tests if we can find video name string in filename
			{
				Debug.Log(file.Name + " contains " + videoName);
				if (extensions.Contains(file.Extension))
				{
					Debug.Log("found thumbnail with extension " + file.Extension);
					return file;
				}	
			}
		}

		Debug.Log("no thumbnail found for " + videoName);
		return new FileSystemEntry();
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
