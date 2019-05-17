using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTrigger : MonoBehaviour {
	
	[SerializeField] string triggerName = "Trigger";
	
	Animator animator = null;
	int triggerId;
	
	void Start(){
		if(animator != null) return;
		animator = GetComponent<Animator>();
		triggerId = Animator.StringToHash(triggerName);
	}
	
	public void Trigger(){
		animator.SetTrigger(triggerId);
	}
	
}
