using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;

public class VideosDisplayer : MonoBehaviour {

	[SerializeField] GameObject videoShelfPrefab;
	[SerializeField] GameObject videoDisplayPrefab;
	[SerializeField] UnityEvent onFoundOneVideo;

	[Serializable]
	public class VideoChoice {
		public string question = "Question";
		public string option1 = "Option 1";
		public string option2 = "Option 2";
		public string video1 = "";
		public string video2 = "";
	}

	[Serializable]
	public class VideoSettings {
		public bool is360 = false;
		public string description = "";
		public string objectsNeeded = "";
		public Vector4[] deltaAngles = new Vector4[0];// x-y-z: euler angles; w: timestamp
		public VideoChoice[] choices = new VideoChoice[0];//can only have 0 or 1 element.

		public override string ToString() {
			return "[VideoSettings: is360=" + is360 + " | description=" + description + " | objectsNeeded=" + objectsNeeded + " | deltaAngles=" + deltaAngles + "]";
		}
	}

	public static VideosDisplayer Instance { get; private set; }

	public List<VideoDisplay> Displays { get { return displayedVideos; } }

	List<VideoDisplay> displayedVideos = new List<VideoDisplay>();
	GameObject lastVideoShelf = null;

	public void AddVideo(string path) {
		Haze.Logger.Log("Adding video file: " + path + "...");
		try {
			//Extract filename
			string[] split = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			string directory = "";
			for(int i = 0; i < split.Length - 1; ++i)
				directory += split[i] + "/";
			split = split[split.Length - 1].Split(new string[] { ".mp4" }, StringSplitOptions.RemoveEmptyEntries);
			string filename = "";
			foreach(string f in split) {
				filename += f + ".";
			}
			filename = filename.Substring(0, filename.Length - 1);

			//Is it a guide?
			split = filename.Split(new string[] { "Guide", "guide", "GUIDE" }, StringSplitOptions.None);
			if(split.Length > 1) {
				//ok! this is a guide video.
				string videoName = "";
				foreach(string f in split) {
					videoName += f;
				}

				//Try to find the associated settings file
				string settingsPath = directory + videoName + "_Settings.json";
				VideoSettings settings;
				if(File.Exists(settingsPath)) {
					string json = File.ReadAllText(settingsPath);
					settings = JsonUtility.FromJson<VideoSettings>(json);
				} else {
					//try to find it in the persistent data path instead of the sd card root then maybe?
					settingsPath = Application.persistentDataPath + videoName + "_Settings.json";
					if(File.Exists(settingsPath)) {
						string json = File.ReadAllText(settingsPath);
						settings = JsonUtility.FromJson<VideoSettings>(json);
					} else {
						//no settings for this video yet.
						settings = new VideoSettings();
						Haze.Logger.Log("Saving settings...");
						SaveVideoSettings(path, videoName, settings);
					}
				}

				Haze.Logger.Log("Displaying video: " + videoName);

				if(lastVideoShelf == null || lastVideoShelf.transform.GetChild(lastVideoShelf.transform.childCount - 1).childCount >= 3) {
					lastVideoShelf = Instantiate(videoShelfPrefab, transform);//add a shelf
				}
				displayedVideos.Add(Instantiate(videoDisplayPrefab, lastVideoShelf.transform.GetChild(lastVideoShelf.transform.childCount - 1)).GetComponent<VideoDisplay>().Init(path, videoName, settings));

				if(displayedVideos.Count == 1)
					onFoundOneVideo.Invoke();

			} else {
				//Not a guide. ignore it.
			}
		}catch(Exception e) {
			//could silently ignore, probably
			Haze.Logger.LogWarning("Video " + path + " cannot be added: " + e);
		}
	}

	public void SaveVideoSettings(string videoPath, string videoName, VideoSettings settings) {
#if UNITY_ANDROID && !UNITY_EDITOR
		//save directory is persistent data path
		string directory = Application.persistentDataPath;
#else
		//save directory is sd card root
		string[] split = videoPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
		string directory = "";
		for(int i = 0; i<split.Length-1; ++i)
			directory += split[i] + "/";
#endif

		//Save settings to json file
		string json = JsonUtility.ToJson(settings);
		//File.WriteAllText(directory + videoName + "_Settings.json", json);
		FileWriter.WriteFile(directory, videoName + "_Settings.json", json);
	}

	public void OnPairConnection(TCPConnection connection) {
		foreach(VideoDisplay videoDisplay in displayedVideos) {
			GuideAdapter adapter = GuideAdapter.Instance;
			if(adapter) {
				adapter.SendHasVideo(connection, videoDisplay.VideoName);
			}
		}
	}

	public VideoDisplay FindVideo(string videoName) {
		foreach(VideoDisplay v in displayedVideos) {
			if(v.VideoName == videoName)
				return v;
		}
		return null;
	}

	private void Start() {
		Instance = this;
	}

	private void OnDestroy() {
		Instance = null;
	}

	private void Update() {
		//check whether we have all the videos on each device connected to us
		foreach(VideoDisplay videoDisplay in displayedVideos) {
			string videoName = videoDisplay.VideoName;
			ConnectionsDisplayer cd = ConnectionsDisplayer.Instance;
			bool allConnectedDevicesHaveIt = true;
			int numberOfConnectedDevices = 0;
			if(cd != null) {
				foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in cd.Handles) {
					if(handle.connection.paired) {
						++numberOfConnectedDevices;
						if(!handle.display.VideosAvailable.Contains(videoName)) {
							allConnectedDevicesHaveIt = false;
							//if(videoDisplay == VideoDisplay.expandedDisplay)
							//	videoDisplay.contract();//no longer available. //<- we do not need to do this anymore. Simply greying out the "Choose" button is enough.
							break;
						}
					}
				}
			}

			bool previouslyAvailable = videoDisplay.Available;
			videoDisplay.Available = allConnectedDevicesHaveIt;

			if(videoDisplay.Available && numberOfConnectedDevices > 1 && videoDisplay.Settings.choices.Length > 0) {
				videoDisplay.Available = false;//Cannot display a video with choices if there's more than one device connected yet!
			}

			//Close the expanded display if its no longer available...
			/*if(VideoDisplay.expandedDisplay == videoDisplay && !videoDisplay.Available) {
				videoDisplay.contract();
			}*/
			//Nope - instead simply update it
			if(VideoDisplay.expandedDisplay == videoDisplay && previouslyAvailable != videoDisplay.Available)//only update if its availability changed this frame
				videoDisplay.expand();
		}
	}

}
