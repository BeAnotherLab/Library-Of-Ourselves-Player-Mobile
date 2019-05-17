using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/* Hierarchy:
--Joystick
 |--Center (Image)
 | |--Target (Image)
 | |--Line (Image)
*/

public class Joystick : MonoBehaviour {
	
	[SerializeField] Transform center;//the center of the joystick; appears upon touch
	[SerializeField] float maxDelta = 100;
	[SerializeField] bool visible = true;
	
	Vector2 axes = Vector2.zero;
	public Vector2 Axes{
		get{ return axes; }
	}
	
	public bool Visible{
		get{ return visible; }
		set{ visible = value; UpdateVisibility(); }
	}
	
	GameObject centerGO;
	Transform target;
	RectTransform line;
	float oneOverCanvasScale;
	float oneOverMaxDelta;
	
	Vector2 delta;
	
	void Start(){
		centerGO = center.gameObject;
		centerGO.SetActive(false);
		target = center.GetChild(0);
		line = center.GetChild(1).GetComponent<RectTransform>();
		oneOverCanvasScale = 1.0f/((Canvas)FindObjectOfType(typeof(Canvas))).transform.localScale.x;
		oneOverMaxDelta = 1/maxDelta;
		
		UpdateVisibility();
	}
	
	void Update(){
		
		if(Time.timeScale == 0) return;
		
		if(Touching){
			if(!centerGO.activeSelf){
				centerGO.SetActive(true);//show the cursor
				center.position = CursorPos;
			}
			
			Vector2 origin = new Vector2(center.position.x, center.position.y);
			delta = (CursorPos - origin) * oneOverCanvasScale;
			//cap delta
			delta = Vector2.ClampMagnitude(delta, maxDelta);
			
			target.localPosition = delta;
			
			//render the line from (0,0,0) to target.localPosition
			line.sizeDelta = new Vector2(delta.magnitude, line.sizeDelta.y);
			//rotate the line so that it points towards delta
			line.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
			
			axes = delta*oneOverMaxDelta;//now it's from 0-1
			
		}else if(centerGO.activeSelf){
			centerGO.SetActive(false);//not touching, let's hide ourselves
			axes = Vector2.zero;
		}
		
	}
	
	/* true if the user is currently touching the screen (or mouse clicked) */
	bool Touching{
		get{
			return Input.GetMouseButton(0);
		}
	}
	
	Vector2 CursorPos{
		get{
			return Input.mousePosition;
		}
	}
	
	void UpdateVisibility(){
		
		centerGO.GetComponent<Image>().enabled = visible;
		target.GetComponent<Image>().enabled = visible;
		line.GetComponent<Image>().enabled = visible;
		
	}
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(Joystick))]
public class JoystickEditor : Editor {
	
	public override void OnInspectorGUI(){
		Joystick j = (Joystick)target;
		
		DrawDefaultInspector();
		
		Vector2 axes = j.Axes;
		EditorGUILayout.LabelField("Axes", "("+axes.x+", "+axes.y+")");
		
		//repaint next frame:
		EditorUtility.SetDirty(target);
    }
	
}
#endif