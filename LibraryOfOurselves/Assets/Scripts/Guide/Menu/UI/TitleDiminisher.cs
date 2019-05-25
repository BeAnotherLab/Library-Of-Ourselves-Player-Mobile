using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TitleDiminisher : MonoBehaviour{

	[SerializeField] ScrollRect scrollrect;
	[SerializeField] float maxScroll = 300;
	[SerializeField] Interpolation.EasingFunction easingFunction;
	[SerializeField] FloatEvent onValueUpdate;

	float currentY = 0;
	Interpolation.Ease ease;

	private void Start() {

		ease = Interpolation.assignEasingFunction(easingFunction);

		scrollrect.onValueChanged.AddListener(delegate (Vector2 vec2) {
			if(scrollrect.verticalScrollbar.gameObject.activeSelf) {
				float y = scrollrect.content.rect.height - vec2.y * scrollrect.content.rect.height;
				y = Mathf.Clamp(y, 0, maxScroll) / maxScroll;
				if(y != currentY) {
					onValueUpdate.Invoke(ease(y));
					currentY = y;
				}
			} else if(currentY != 0){
				onValueUpdate.Invoke(0);
				currentY = 0;
			}
		});
	}

}
