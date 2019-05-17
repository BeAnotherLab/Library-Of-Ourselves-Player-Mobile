using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnKeypress : MonoBehaviour {

    [SerializeField] List<KeyCode> keys = new List<KeyCode>() { KeyCode.Return };
    [SerializeField] UnityEvent onPress;

	void Update () {
        foreach (KeyCode key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                onPress.Invoke();
            }
        }
	}
}
