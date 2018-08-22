using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsSaved : MonoBehaviour {
	
	[SerializeField] AdvancedSettings settings;
	[SerializeField] UnityEvent saved;
	[SerializeField] UnityEvent notSaved;
	
	public void Check(){
		if(settings.isSaved()){
			saved.Invoke();
		}else{
			notSaved.Invoke();
		}
	}
	
}
