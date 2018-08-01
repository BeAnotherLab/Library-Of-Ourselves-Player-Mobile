using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIVideoPlacer : MonoBehaviour {
	
	const string PREFIX_360 = "360_";//any file that starts with this name on the guide app will be considered a 360 video
	
	[SerializeField] GameObject prefabVideoUI;
	[SerializeField] Transform videoRoot;
	
	public void AddVideo(string path){
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
		
		ui.name = videoName;
		
		ui.GetComponent<MainMenuUIVideo>().Init(videoName, path, is360);
	}
	
}
