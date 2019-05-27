using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haze{
	[ExecuteInEditMode]
	public class KeepDistanceFrom : MonoBehaviour{
		
		[SerializeField] Transform target = null;
		[SerializeField] bool targetIsAboveSibling = false;
		[SerializeField] float distance = 1;
		
		void OnEnable(){
			if(targetIsAboveSibling){
				int index = transform.GetSiblingIndex();
				if(index > 0)
					target = transform.parent.GetChild(index-1);
			}
		}
		
		void Update(){
			if(target != null){
				Vector3 toTarget = target.position - transform.position;
				toTarget.Normalize();
				transform.position = target.position - toTarget * distance;
			}
		}
		
	}
}
