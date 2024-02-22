using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class VideoNamesDropdown : MonoBehaviour{

	[SerializeField] private bool _ignoredCurrentlyExpandedDisplay = true;

	Dropdown dropdown;

	private void OnEnable() { //TODO shouldn't it be in Awake() ?
		dropdown = GetComponent<Dropdown>();

		dropdown.options = new List<Dropdown.OptionData>();
		foreach (VideoDisplay vd in VideosDisplayer.Instance.Displays) {
			if (vd != VideoDisplay.expandedDisplay || !_ignoredCurrentlyExpandedDisplay) {
				dropdown.options.Add(new Dropdown.OptionData(vd.VideoName));
			}
		}
	}

	public string Selected {
		get {
			return dropdown.options[dropdown.value].text;
		}
		set {
			SetSelected(value);
		}
	}
	
	private void SetSelected(string videoName) {
		for (int index = 0; index < dropdown.options.Count; ++index) {
			if (dropdown.options[index].text == videoName) {
				dropdown.value = index;
				dropdown.captionText.text = videoName;
				return;
			}
		}
		dropdown.value = 0;
		if (dropdown.options.Count <= 0) dropdown.captionText.text = "[???]";
		else dropdown.captionText.text = dropdown.options[0].text;
	}
}
