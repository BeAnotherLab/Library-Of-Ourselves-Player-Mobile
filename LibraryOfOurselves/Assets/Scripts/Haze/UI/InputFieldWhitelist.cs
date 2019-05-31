using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(InputField))]
public class InputFieldWhitelist : MonoBehaviour{

	[SerializeField] string whitelist = "0123456789";
	[SerializeField] string allowInFirstPlace = "-";
	[SerializeField] string allowExceptFirst = ".";
	[SerializeField] bool parseToInt = false;
	[SerializeField] Vector2Int intRange = new Vector2Int(-10, 10);
	[SerializeField] bool parseToFloat = false;
	[SerializeField] Vector2 floatRange = new Vector2(-1, 1);

	[SerializeField] UnityEvent onInputCorrect;
	[SerializeField] UnityEvent onInputIncorrect;

	public bool Correct { get; private set; }

	public string StringValue { get; private set; }

	public int IntValue { get; private set; }

	public float FloatValue { get; private set; }

	/// Bound to the InputField's OnValueChange event
	public void OnValueChanged(string t) {

		bool inputOk = true;
		bool changed = false;

		//whitelist characters away
		{
			string newVal = "";
			for(int i = 0; i < t.Length; ++i) {
				if(whitelist.Contains("" + t[i])) {
					newVal += t[i];
				} else if(i == 0 && allowInFirstPlace.Contains("" + t[i])) {
					newVal += t[i];
				} else if(i > 0 && allowExceptFirst.Contains("" + t[i])) {
					newVal += t[i];
				} else {
					changed = true;
				}
			}
			StringValue = newVal;
		}

		//attempt to parse to int?
		if(parseToInt) {
			int ret;
			if(int.TryParse(StringValue, out ret)) {
				if(ret < intRange.x || ret > intRange.y) {
					if(ret < intRange.x) ret = intRange.x;
					else ret = intRange.y;
					changed = true;
					StringValue = "" + ret;
				}
				IntValue = ret;
			} else {
				//Not ok...
				inputOk = false;
			}
		}

		//attempt to parse to float?
		if(parseToFloat) {
			float ret;
			if(float.TryParse(StringValue, out ret)) {
				if(ret < floatRange.x || ret > floatRange.y) {
					if(ret < floatRange.x) ret = floatRange.x;
					else ret = floatRange.y;
					changed = true;
					StringValue = "" + ret;
				}
				FloatValue = ret;
			} else {
				//Not ok...
				inputOk = false;
			}
		}

		if(changed)
			GetComponent<InputField>().text = StringValue;

		if(Correct != inputOk) {
			if(inputOk) onInputCorrect.Invoke();
			else onInputIncorrect.Invoke();
		}
		Correct = inputOk;
	}

}
