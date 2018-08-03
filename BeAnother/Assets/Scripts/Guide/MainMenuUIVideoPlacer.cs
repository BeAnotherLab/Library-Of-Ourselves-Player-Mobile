using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIVideoPlacer : MonoBehaviour {
	
	const string PREFIX_360 = "360_";//any file that starts with this name on the guide app will be considered a 360 video
	static readonly string[] SUFFIXES = new string[]{"_guide", "Guide", "guide"};//will strip these out from the end of the displayed name
	
	[SerializeField] GameObject prefabVideoUI;
	[SerializeField] Transform videoRoot;
	
	public void AddVideo(string path){
		//check whether it is a 'guide' video:
		bool isGuide = false;
		foreach(string suffix in SUFFIXES){
			if(path.EndsWith(suffix + ".mp4")){
				isGuide = true;
				break;
			}
		}
		if(!isGuide) return;
		
		GameObject ui = Instantiate(prefabVideoUI, videoRoot);
		string[] slashes = path.Split('/');
		slashes = slashes[slashes.Length-1].Split('\\');
		string videoName = slashes[slashes.Length-1];
		string[] dots = videoName.Split('.');
		videoName = "";
		for(int i = 0; i<dots.Length-1; ++i)
			videoName += dots[i];
		
		//check 360
		bool is360 = false;
		if(videoName.StartsWith(PREFIX_360)){
			is360 = true;
			//take the leading "360_" off the display name
			videoName = videoName.Substring(PREFIX_360.Length, videoName.Length-PREFIX_360.Length);
		}
		
		//get rid of suffix "_guide", "Guide" or "guide"
		foreach(string suffix in SUFFIXES){
			if(videoName.EndsWith(suffix)){
				videoName = videoName.Substring(0, videoName.Length - suffix.Length);
			}
		}
		
		ui.name = videoName;
		
		ui.GetComponent<MainMenuUIVideo>().Init(videoName, path, is360);
	}
	
}
