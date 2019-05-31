using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleOrientationEditor : MonoBehaviour{

	[SerializeField] InputFieldWhitelist timeInput;
	[SerializeField] InputFieldWhitelist pitchInput;
	[SerializeField] InputFieldWhitelist yawInput;
	[SerializeField] InputFieldWhitelist rollInput;

	public void OnClickDelete() {
		Destroy(gameObject);
	}

	public Vector4 DeltaAngle {
		get {
			Vector4 ret = Vector4.zero;
			if(pitchInput.Correct) ret.x = pitchInput.FloatValue;
			if(yawInput.Correct) ret.y = yawInput.FloatValue;
			if(rollInput.Correct) ret.z = rollInput.FloatValue;
			if(timeInput.Correct) ret.w = timeInput.FloatValue;
			return ret;
		}
		set {
			pitchInput.GetComponent<InputField>().text = "" + value.x;
			yawInput.GetComponent<InputField>().text = "" + value.y;
			rollInput.GetComponent<InputField>().text = "" + value.z;
			timeInput.GetComponent<InputField>().text = "" + value.w;
		}
	}

}
