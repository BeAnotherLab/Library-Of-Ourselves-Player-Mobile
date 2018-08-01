using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Video;

[RequireComponent(typeof(Sender))]
public class VideoSlave : MonoBehaviour {
	
	[SerializeField] VideoPlayer spherePlayer;
	[SerializeField] VideoPlayer semispherePlayer;
	[SerializeField] bool bypassBinauralAudio = false;
	
	Sender respond;
	
	VideoSettings currentSettings = null;
	VideoPlayer m_currentPlayer = null;
	
	//The VideoPlayer we're currently using, or null if no video is playing.
	VideoPlayer CurrentPlayer{
		get{ return m_currentPlayer; }
		set{
			if(m_currentPlayer != null){
				m_currentPlayer.loopPointReached -= OnVideoEnd;
				m_currentPlayer.gameObject.SetActive(false);
				if(!bypassBinauralAudio)
					m_currentPlayer.GetComponent<BinauralAudio>().Stop();
			}
			if(value != null){
				value.gameObject.SetActive(true);
				value.loopPointReached += OnVideoEnd;
			}else{
				currentSettings = null;
			}
			m_currentPlayer = value;
		}
	}
	
	class VideoSettings{
		private string name;
		private float pitch = 0, yaw = 0, roll = 0;
		private bool is360 = false;
		private bool is360Set = false;
		
		public VideoSettings(string n){
			name = n;
			if(PlayerPrefs.HasKey(name+"x"))
				pitch = PlayerPrefs.GetFloat(name+"x");
			if(PlayerPrefs.HasKey(name+"y"))
				yaw = PlayerPrefs.GetFloat(name+"y");
			if(PlayerPrefs.HasKey(name+"z"))
				roll = PlayerPrefs.GetFloat(name+"z");
			if(PlayerPrefs.HasKey(name+"i")){
				is360 = PlayerPrefs.GetInt(name+"i") == 1;
				is360Set = true;
			}
		}
		
		public float Pitch{
			get{
				return pitch;
			}
			set{
				if(pitch == value) return;
				pitch = value;
				write();
			}
		}
		
		public float Yaw{
			get{
				return yaw;
			}
			set{
				if(yaw == value) return;
				yaw = value;
				write();
			}
		}
		
		public float Roll{
			get{
				return roll;
			}
			set{
				if(roll == value) return;
				roll = value;
				write();
			}
		}
		
		public bool Is360{
			get{
				return is360;
			}
			set{
				if(is360 == value) return;
				is360 = value;
				write();
			}
		}
		
		public bool Is360Set{//whether this video's 360v235 is overriden
			get{ return is360Set; }
		}
		
		public string Name{
			get{ return name; }
		}
		
		public void applyRotations(VideoPlayer current){
			if(current != null)
				current.GetComponent<PlayerRotations>().Correct(pitch, yaw, roll);
		}
		
		void write(){
			PlayerPrefs.SetFloat(name+"x", pitch);
			PlayerPrefs.SetFloat(name+"y", yaw);
			PlayerPrefs.SetFloat(name+"z", roll);
			PlayerPrefs.SetInt(name+"i", is360? 1 : 0);
		}
	}
	
	void Start(){
		respond = GetComponent<Sender>();
		spherePlayer.gameObject.SetActive(false);
		semispherePlayer.gameObject.SetActive(false);
	}
	
	void LoadVideo(string name, string mode){
		//find mp4 file:
		string path = Filesystem.SDCardRoot + name + ".mp4";
		print("Fetching " + path);
		if(!File.Exists(path)){
			print("Video file " + path + " does not exist.");
			respond.Send("filenotfound "+path);
			return;
		}
		
		//find audio files:
		string audioPath = Filesystem.SDCardRoot + name;
		if(!bypassBinauralAudio && !BinauralAudio.Exists(audioPath)){
			respond.Send("filenotfound "+audioPath);
			return;
		}
		
		//get settings
		currentSettings = new VideoSettings(name);
		if(!currentSettings.Is360Set)
			currentSettings.Is360 = mode == "360";
		
		//load the video into the correct player
		CurrentPlayer = currentSettings.Is360 ? spherePlayer : semispherePlayer;
		CurrentPlayer.url = path;
		CurrentPlayer.Prepare();
		
		//load the audio:
		if(!bypassBinauralAudio)
			CurrentPlayer.GetComponent<BinauralAudio>().Load(audioPath);
		
		//recalibrate
		Calibrate();
		currentSettings.applyRotations(CurrentPlayer);
	}
	
	void Play(){
		if(CurrentPlayer == null) return;
		CurrentPlayer.Play();
		if(!bypassBinauralAudio)
			CurrentPlayer.GetComponent<BinauralAudio>().Play();
	}
	
	void Pause(){
		if(CurrentPlayer == null) return;
		CurrentPlayer.Pause();
		if(!bypassBinauralAudio)
			CurrentPlayer.GetComponent<BinauralAudio>().Pause();
	}
	
	void Stop(){
		CurrentPlayer = null;
	}
	
	void Calibrate(){
		if(CurrentPlayer != null) CurrentPlayer.GetComponent<PlayerRotations>().Calibrate();
	}
	
	public void OnReceived(string data){
		string[] dat = data.Split(' ');
		
		if(dat.Length < 1) return;
		
		if(dat[0] == "select"){//Load a video; argument 1 is filename, 2 is either "360" or "235"
			if(dat.Length > 2)
				LoadVideo(dat[1], dat[2]);
			else
				print("Expecting a 2 arguments in select");
		}else if(dat[0] == "play"){//Start playing the video
			Play();
		}else if(dat[0] == "pause"){//Pauses
			Pause();
		}else if(dat[0] == "stop"){//Unloads the video
			Stop();
		}else if(dat[0] == "calibrate"){//Recenters sphere
			Calibrate();
		}else if(dat[0] == "rotate"){//changes default pitch-yaw-roll (args 1 2 and 3)
			if(currentSettings == null) return;
			if(dat.Length > 3){
				currentSettings.Pitch = (float)Convert.ToDouble(dat[1]);
				currentSettings.Yaw = (float)Convert.ToDouble(dat[2]);
				currentSettings.Roll = (float)Convert.ToDouble(dat[3]);
				currentSettings.applyRotations(CurrentPlayer);
			}else{
				print("Expecting 3 arguments in rotate");
			}
		}else if(dat[0] == "mode"){//changes default mode for this video's playback (arg 1 is either "360" or "235") -- WARNING: reloads video
			if(currentSettings == null) return;
			if(dat.Length > 1){
				currentSettings.Is360 = dat[1] == "360";
				//reload video
				LoadVideo(currentSettings.Name, dat[1]);
			}else{
				print("Expecting 1 argument in mode");
			}
		}else if(dat[0] == "logs"){//sends back console contents
			respond.Send(UIConsole.Logs());
		}else{
			print("Unrecognized message: " + data);
			respond.Send("unknownerror unrecognized message " + data);
		}
		
	}
	
	void OnVideoEnd(VideoPlayer unused){
		Stop();
	}
	
}
