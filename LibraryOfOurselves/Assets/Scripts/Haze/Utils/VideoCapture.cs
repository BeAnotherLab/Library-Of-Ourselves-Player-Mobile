using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VideoCapture : MonoBehaviour{
	
	#if UNITY_EDITOR
	[SerializeField][Range(1, 120)] int frameRate = 25;
	[SerializeField] int superSize = 1;
	[SerializeField] bool recordOnAwake = false;
	[SerializeField] string path = "HazeVideoCapture";
	
	[HideInInspector] public int recording;//elapsed frames, or -1 if not recording
	
	string currentDir = "";
	
	void Awake(){
		recording = -1;
		if(recordOnAwake){
			Play();
		}
	}
	
	public void Play(){
		recording = 0;
        Time.captureFramerate = frameRate;
		//Create folder
		currentDir = path + "-" + System.DateTime.Now.ToString("yyyyddMMhhmmss");
		System.IO.Directory.CreateDirectory(currentDir);
	}
	
	public void Stop(){
		recording = -1;
		Time.captureFramerate = 0;
	}
	
	void Update(){
		if(recording >= 0){
			
			string filename = currentDir + "/" + string.Format("{0:D04}", recording) + ".png";
			ScreenCapture.CaptureScreenshot(filename, superSize);
			
			++recording;
		}
	}
	#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(VideoCapture))]
public class VideoCaptureEditor : Editor{
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		VideoCapture vcap = target as VideoCapture;
		if(vcap && EditorApplication.isPlaying){
			if(vcap.recording < 0){
				if(GUILayout.Button("Start recording")){
					vcap.Play();
				}
			}else{
				GUILayout.Label("Recording... (" + vcap.recording + " frames)");
				if(GUILayout.Button("Stop recording")){
					vcap.Stop();
				}
			}
		}
	}
}
#endif