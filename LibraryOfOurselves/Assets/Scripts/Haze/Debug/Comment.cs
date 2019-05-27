using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comment : MonoBehaviour {
	#if UNITY_EDITOR
	[TextArea]
	[Tooltip("Doesn't do anything. Just comments shown in inspector")]
	public string notes = "";
	#endif
}
