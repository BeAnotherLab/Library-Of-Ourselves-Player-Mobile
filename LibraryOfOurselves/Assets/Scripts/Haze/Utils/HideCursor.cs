using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCursor : MonoBehaviour
{
    void Awake() {
		Cursor.visible = false;
	}
}
