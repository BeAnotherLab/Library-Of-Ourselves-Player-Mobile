using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfRelativePosition : MonoBehaviour {
	
	[SerializeField] RelativePositioning onlyShowIf = RelativePositioning.ToTheRight;
	[SerializeField] Behaviour toShow;
	[SerializeField] Transform relative;
	
	enum RelativePositioning{
		ToTheRight,
		ToTheLeft,
		Above,
		Under
	}
	
	Transform t;
	
	void Start(){
		t = transform;
	}
	
	void Update(){
		switch(onlyShowIf){
			case RelativePositioning.ToTheRight:
				toShow.enabled = relative.position.x > t.position.x;
				break;
			case RelativePositioning.ToTheLeft:
				toShow.enabled = relative.position.x < t.position.x;
				break;
			case RelativePositioning.Above:
				toShow.enabled = relative.position.y > t.position.y;
				break;
			case RelativePositioning.Under:
				toShow.enabled = relative.position.y < t.position.y;
				break;
		}
	}
	
}
