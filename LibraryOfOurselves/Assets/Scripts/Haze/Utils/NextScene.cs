using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NextScene : MonoBehaviour {
	
	[SerializeField] SceneField scene;
	public bool showAd = false;//feel free to change from another script before calling Next()
	[SerializeField] bool verbose = false;
	
	public void Next(){
		if(verbose) print("Going to scene " + scene);
		if(showAd){
			#if UNITY_ADS
			StartCoroutine(showAdThenNext(scene));
			#else
			Debug.LogError("You need to enable Unity Ads to show ads here!");
			#endif
		}else{
			SceneManager.LoadScene(scene);
		}
	}
	
	#if UNITY_ADS
	IEnumerator showAdThenNext(SceneField scene){
		if(Advertisement.isSupported && Advertisement.isInitialized && Advertisement.IsReady()){
			//stop audio
			float volume = AudioListener.volume;
			AudioListener.volume = 0;
			//show ad
			Advertisement.Show();
			while(Advertisement.isShowing){
				yield return null;//wait until the user finishes up with this ad
			}
			//restart audio
			AudioListener.volume = volume;
		}
		SceneManager.LoadScene(scene);
	}
	#endif
	
}

#if UNITY_EDITOR

[CustomEditor(typeof(NextScene))]
public class NextSceneEditor : Editor{
	public override void OnInspectorGUI(){
		NextScene ns = (NextScene) target;
		
		DrawDefaultInspector();
		
		if(GUILayout.Button("Next")){
			ns.Next();
		}
		
	}
}

#endif

