using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NextScene : MonoBehaviour {
	
	[SerializeField] SceneField scene;
	
	public void Next(){
		SceneManager.LoadScene(scene);
	}
	
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

