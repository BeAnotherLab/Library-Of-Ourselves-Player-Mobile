﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.Events;


[Serializable]
public class VideoChoice { //TODO rename to option?
	public string description = "description";  
	public string video = ""; //the destination video
	public Vector3 position = new Vector3();
} //TODO move to own file

[Serializable]
public class VideoSettings {
	public string description = "";
	public string objectsNeeded = "";
	public bool is360 = false;
	public string difficulty = "";
	public Vector4[] deltaAngles = new Vector4[0];// x-y-z: euler angles; w: timestamp
	//public VideoChoice[] choices = new VideoChoice[0];//can only have 0 or 1 element.
	public List<VideoChoice> choices = new List<VideoChoice>();

	public override string ToString() {
		return "[VideoSettings: is360=" + is360 + " | description=" + description + " | objectsNeeded=" + objectsNeeded + " | deltaAngles=" + deltaAngles + "]";
	}
} //TODO own file

public class VideosDisplayer : MonoBehaviour { //displays list of videos in a grid and saves settings

	[SerializeField] GameObject videoShelfPrefab;
	[SerializeField] GameObject videoDisplayPrefab;
	[SerializeField] UnityEvent onFoundOneVideo;

	public static VideosDisplayer Instance { get; private set; }

	public List<VideoDisplay> Displays { get { return displayedVideos; } }

	List<VideoDisplay> displayedVideos = new List<VideoDisplay>();
	GameObject lastVideoShelf;

	private void OnEnable()
	{
		FileBrowserTest.RootFolderPicked += AddVideos;
	}

	private void OnDisable()
	{
		FileBrowserTest.RootFolderPicked += AddVideos;
	}

	private void Start() {
		Instance = this; //TODO remove singleton antipattern
	}

	private void OnDestroy() {
		Instance = null; //TODO remove singleton antipattern
	}

	private void Update() {
		//check whether we have all the videos on each device connected to us
		foreach (VideoDisplay videoDisplay in displayedVideos) {
			string videoName = videoDisplay.VideoName;
			ConnectionsDisplayer cd = ConnectionsDisplayer.Instance;
			bool allConnectedDevicesHaveIt = true;
			int numberOfConnectedDevices = 0;
			if (cd != null) {
				foreach (ConnectionsDisplayer.DisplayedConnectionHandle handle in cd.Handles) {
					if(handle.connection.paired) {
						++numberOfConnectedDevices;
						if(!handle.display.VideosAvailable.Contains(videoName)) {
							allConnectedDevicesHaveIt = false;
							break;
						}
					}
				}
			}

			bool previouslyAvailable = videoDisplay.Available;
			videoDisplay.Available = allConnectedDevicesHaveIt;

			if(videoDisplay.Available && numberOfConnectedDevices > 1 && videoDisplay.Settings.choices.Count > 0) {
				videoDisplay.Available = false;//Cannot display a video with choices if there's more than one device connected yet!
			}

			if(VideoDisplay.expandedDisplay == videoDisplay && previouslyAvailable != videoDisplay.Available)//only update if its availability changed this frame
				videoDisplay.Expand();
		}
	}

	public void AddVideos()
	{
		Debug.Log("Checking for files in " + DataFolder.GuidePath);
			
		foreach (FileSystemEntry file in FileBrowserHelpers.GetEntriesInDirectory(DataFolder.GuidePath, true))
		{
			Debug.Log("in this folder there is " + file.Path);
			if (file.Extension == ".mp4") Haze.Logger.Log("Found " + file.Path);
		
			Haze.Logger.Log("Adding video file: " + DataFolder.GuidePath + "...");
		
			string fileName = FileBrowserHelpers.GetFilename(DataFolder.GuidePath);
			
			//Is it a guide?
			if (DataFolder.GuidePath.IndexOf("guide", 0, StringComparison.OrdinalIgnoreCase) != -1) //tests if we can find case independent "guide" string in filename
			{
				//ok! this is a guide video. Try to find the associated settings file
				var videoName = fileName.Substring(0, fileName.IndexOf("guide", StringComparison.OrdinalIgnoreCase));
				string parentDirectory = FileBrowserHelpers.GetDirectoryName(DataFolder.GuidePath);
				Debug.Log("we want to check if we can write in the following directory" + parentDirectory);
				string settingsPath = FileBrowserHelpers.CreateFileInDirectory(parentDirectory, videoName + "_Settings.json");

				VideoSettings settings;
				Debug.Log("Does settings exist ? " + settingsPath);
				
				if (FileBrowserHelpers.FileExists(DataFolder.GuidePath))
				{
					Debug.Log("yes, file exists");
					string json = FileBrowserHelpers.ReadTextFromFile(settingsPath);
					settings = JsonUtility.FromJson<VideoSettings>(json);
					Debug.Log("read from settings file : " + settings);
				}
				else //no settings for this video yet.
				{
					settings = new VideoSettings();
					Haze.Logger.Log("No file found, saving settings...");
					SaveVideoSettings(videoName, settings);
				}

				Haze.Logger.Log("Displaying video: " + fileName);

				if (lastVideoShelf == null || lastVideoShelf.transform.GetChild(lastVideoShelf.transform.childCount - 1).childCount >= 3) {
					lastVideoShelf = Instantiate(videoShelfPrefab, transform);//add a shelf //TODO use rectangular layout so that we can ignore this
				}
				
				//instantiate and initialize new video display
				displayedVideos
					.Add(Instantiate(videoDisplayPrefab, lastVideoShelf.transform.GetChild(lastVideoShelf.transform.childCount - 1))
					.GetComponent<VideoDisplay>()
					.Init(DataFolder.GuidePath, videoName, settings));

				if (displayedVideos.Count == 1) onFoundOneVideo.Invoke();
			}
		}
	}

	//public void AddSettings(string path)
	
	public void SaveVideoSettings(string videoName, VideoSettings settings) { //TODO move to own class
		string json = JsonUtility.ToJson(settings); 
		Debug.Log("will write to " + videoName);
		var parentDirectory = FileBrowserHelpers.GetDirectoryName(DataFolder.GuidePath);
		var newFile = FileBrowserHelpers.CreateFileInDirectory(parentDirectory, videoName + "_Settings.json");
		
		Debug.Log("Writing file : " + newFile);
		FileBrowserHelpers.WriteTextToFile(newFile, json);
	}

	public void OnPairConnection(TCPConnection connection) {
		foreach (VideoDisplay videoDisplay in displayedVideos) {
			GuideAdapter adapter = GuideAdapter.Instance;
			if (adapter) {
				adapter.SendHasVideo(connection, videoDisplay.VideoName);
			}
		}
	}

	public VideoDisplay FindVideo(string videoName) {
		foreach (VideoDisplay v in displayedVideos) {
			if (v.VideoName == videoName)
				return v;
		}
		return null;
	}

}
