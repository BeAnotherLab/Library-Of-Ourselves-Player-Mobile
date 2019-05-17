using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Fader : MonoBehaviour {
	
	[SerializeField] float velocity = 0.5f;
	[SerializeField] int blackFrames = 0;
	[SerializeField] bool onStart = true;
	[SerializeField] UnityEvent onFadedIn;
	[SerializeField] UnityEvent onFadedOut;
    [SerializeField] Color color = new Color(0, 0, 0, 1);
	
	Image image;
    Material material = null;
	
	Coroutine currentRoutine = null;

    float __alpha;
    public float Alpha{
        get{
            return __alpha;
        }
        private set{
            __alpha = value;
            if (image)
                image.color = new Color(color.r, color.g, color.b, value);
            if (material)
                material.color = new Color(color.r, color.g, color.b, value);
        }
    }

	void Start () {
		image = GetComponent<Image>();
        MeshRenderer mesh = GetComponent<MeshRenderer>();
		if(mesh)
			material = mesh.material;
		
		if(onStart){
            Alpha = 1;
			FadeIn();
		}
	}
	
	void interruptAndStart(IEnumerator routine){
		if(currentRoutine != null)
			StopCoroutine(currentRoutine);
		currentRoutine = base.StartCoroutine(routine);
	}
	
	new void StartCoroutine(IEnumerator ie){
		Debug.LogError("Use interruptAndStart instead of StartCoroutine here.");
	}
	
	IEnumerator blackToTransparent(float vel){

        float alpha = Alpha;
		
		for(int i = 0; i<blackFrames; ++i)
			yield return null;
		
		while(alpha > 0){
            Alpha = alpha;
			alpha -= Time.unscaledDeltaTime*vel;
			yield return null;
		}
        Alpha = 0;
		
		gameObject.SetActive(false);//disappear
		
		currentRoutine = null;
		
		onFadedIn.Invoke();
		
	}
	
	IEnumerator transparentToBlack(float vel, UnityEvent onEnd = null){

        float alpha = Alpha;
		
		while(alpha < 1){
            Alpha = alpha;
			alpha += Time.unscaledDeltaTime*vel;
			yield return null;
		}
        Alpha = 1;
		
		currentRoutine = null;
		
		onFadedOut.Invoke();
		
		if(onEnd != null)
			onEnd.Invoke();
		
	}
	
	public void FadeOut(float vel = 0){
		gameObject.SetActive(true);
		interruptAndStart(transparentToBlack(vel == 0 ? velocity : vel));
	}
	
	public void FadeIn(float vel = 0){
		gameObject.SetActive(true);
		interruptAndStart(blackToTransparent(vel == 0 ? velocity : vel));
	}

	public void FadeOutThen(UnityEvent onEnd){
		gameObject.SetActive(true);
		interruptAndStart(transparentToBlack(velocity, onEnd));
	}
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(Fader))]
public class FaderEditor : Editor{

    public override void OnInspectorGUI(){
        Fader fader = target as Fader;

        DrawDefaultInspector();

        EditorGUILayout.LabelField("Alpha = " + fader.Alpha);
    }

}
#endif