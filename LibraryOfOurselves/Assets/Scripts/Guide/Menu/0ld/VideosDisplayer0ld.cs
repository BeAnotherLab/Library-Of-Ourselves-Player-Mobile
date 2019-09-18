
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VideosDisplayer0ld : MonoBehaviour {

	static readonly string[] SUFFIXES = new string[] { "_g", "_G", "_guide", "_Guide", "Guide", "guide", "GUIDE", "_GUIDE" };//will only consider filenames that end with one of those

	[SerializeField] GameObject videoDisplayPrefab;
	[SerializeField] Transform videoRoot;

	[Serializable]
	public class VideoSettings {

		public bool is360 = false;
		public VideoMeta meta = new VideoMeta();

		[Serializable]
		public class VideoMeta {
			public string description = "";
			public string objects = "";
			public float pitch = 0;
			public float yaw = 0;
			public float roll = 0;
		}

		public override string ToString() {
			return "is360: " + is360 + " | description: " + meta.description + " | objects: " + meta.objects + " | pitch: " + meta.pitch + " | yaw: " + meta.yaw + " | roll: " + meta.roll;
		}

		//Build new settings object from old version settings
		public VideosDisplayer.VideoSettings NewSettings {
			get {
				VideosDisplayer.VideoSettings settings = new VideosDisplayer.VideoSettings();
				settings.is360 = is360;
				settings.description = meta.description;
				settings.objectsNeeded = meta.objects;
				settings.deltaAngles = new Vector4[1] { new Vector4(meta.pitch, meta.yaw, meta.roll, 0), };
				return settings;
			}
		}

		public static VideoMeta FromNewSettings(VideosDisplayer.VideoSettings settings) {
			VideoMeta meta = new VideoMeta();
			meta.description = settings.description;
			meta.objects = settings.objectsNeeded;
			Vector4 orientation = settings.deltaAngles.Length > 0 ? settings.deltaAngles[0] : Vector4.zero;
			meta.pitch = orientation.x;
			meta.yaw = orientation.y;
			meta.roll = orientation.z;
			return meta;
		}
	}

	public static VideosDisplayer0ld Instance { get; private set; }

	public List<VideoDisplay> Displays { get { return displayedVideos; } }

	List<VideoDisplay> displayedVideos = new List<VideoDisplay>();

	public void AddVideo(string path) {
		Debug.Log("Adding video file: " + path + "...");
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
			bool isGuide = false;
			foreach(string suffix in SUFFIXES) {
				if(filename.EndsWith(suffix)) {
					isGuide = true;
					break;
				}
			}
			if(isGuide) {
				//ok! this is a guide video.

				//get rid of Guide suffix
				string videoName = filename;
				foreach(string suffix in SUFFIXES) {
					if(videoName.EndsWith(suffix)) {
						videoName = videoName.Substring(0, videoName.Length - suffix.Length);
					}
				}

				VideoSettings settings = new VideoSettings();

				//is it 360 degrees?
				settings.is360 = false;
				if(videoName.StartsWith("360_")) {
					settings.is360 = true;
					//take the leading "360_" off the display name
					videoName = videoName.Substring("360_".Length, videoName.Length - "360_".Length);
					Debug.Log(videoName + " is 360.");
				} else {
					Debug.Log(videoName + " is 235.");
				}

				//Try to find the associated settings file
				string metaPath = directory + videoName + "Info.json";
				Debug.Log("Attempting to find metadata file in: " + metaPath);
				if(File.Exists(metaPath)) {
					string json = File.ReadAllText(metaPath);
					settings.meta = JsonUtility.FromJson<VideoSettings.VideoMeta>(json);
					Debug.Log("Found metadata file. " + settings.ToString());
				} else {
					//try to find it in the persistent data path instead of the sd card root then maybe?
					metaPath = Application.persistentDataPath + videoName + "Info.json";
					Debug.Log("Not found. Attempting to find metadata file in: " + metaPath);
					if(File.Exists(metaPath)) {
						string json = File.ReadAllText(metaPath);
						settings.meta = JsonUtility.FromJson<VideoSettings.VideoMeta>(json);
						Debug.Log("Found metadata file. " + settings.ToString());
					} else {
						//no settings for this video yet.
						settings.meta = new VideoSettings.VideoMeta();
						Debug.Log("Saving metadata... " + settings.ToString());
						SaveVideoMeta(path, videoName, settings.meta);
					}
				}

				Debug.Log("Displaying video: " + videoName);

				GameObject display = Instantiate(videoDisplayPrefab, videoRoot);
				displayedVideos.Add(display.GetComponent<VideoDisplay>().Init(path, videoName, settings.NewSettings));

			} else {
				//Not a guide. ignore it.
				Debug.Log("Video " + path + " is not a guide video.");
			}
		} catch(Exception e) {
			//could silently ignore, probably
			Debug.LogWarning("Video " + path + " cannot be added: " + e);
		}
	}

	public void SaveVideoMeta(string videoPath, string videoName, VideoSettings.VideoMeta meta) {
#if UNITY_ANDROID && !UNITY_EDITOR
		//save directory is persistent data path
		string directory = Application.persistentDataPath;
#else
		//save directory is sd card root
		string[] split = videoPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
		string directory = "";
		for(int i = 0; i < split.Length - 1; ++i)
			directory += split[i] + "/";
#endif

		//Save settings to json file
		string json = JsonUtility.ToJson(meta);
		//File.WriteAllText(directory + videoName + "Info.json", json);
		FileWriter.WriteFile(directory, videoName + "Info.json", json);

		Debug.Log("Wrote metadata file to: " + directory + videoName + "Info.json");
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
