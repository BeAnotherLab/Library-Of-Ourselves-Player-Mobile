using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Haze{
	[RequireComponent(typeof(LineRenderer))]
	public class LineRenderedCircle : MonoBehaviour{
		
		[SerializeField] bool onStart = false;
		[SerializeField] int vertices = 50;
		[SerializeField] float radius = 1;
		
		void Start(){
			if(onStart) Go();
		}
		
		public void Go(){
			LineRenderer renderer = GetComponent<LineRenderer>();
			renderer.positionCount = vertices;
			
			for(int i = 0; i<vertices; ++i){
				float theta = (float)i/(float)vertices * 2 * Mathf.PI;
				Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0);
				renderer.SetPosition(i, pos);
			}
		}
		
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(LineRenderedCircle))]
	public class LineRenderedCircleEditor : Editor{
		public override void OnInspectorGUI(){
			DrawDefaultInspector();
			LineRenderedCircle circle = target as LineRenderedCircle;
			if(circle && GUILayout.Button("Generate")){
				Undo.RecordObject(circle, "Circle generation");
				circle.Go();
			}
		}
	}
	#endif
}