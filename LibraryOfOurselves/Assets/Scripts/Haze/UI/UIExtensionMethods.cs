using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIExtensionMethods {
	
	/** Waits one frame, then scrolls to the bottom (useful if you're adding stuff to a scroll rect and want it to scroll down) */
	public static void ScrollDownAfterOneFrame(this ScrollRect scrollRect){
		scrollRect.StartCoroutine(scrollRect.ScrollDownAfterOneFrameCoroutine());
	}
	
	public static IEnumerator ScrollDownAfterOneFrameCoroutine(this ScrollRect scrollRect){
		yield return new WaitForEndOfFrame();
		scrollRect.verticalNormalizedPosition = 0;
	}
	
}
