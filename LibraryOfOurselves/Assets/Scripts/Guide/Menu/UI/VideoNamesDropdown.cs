using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class VideoNamesDropdown : MonoBehaviour{

	[SerializeField] private bool _ignoredCurrentlyExpandedDisplay = true;

	private Dropdown _dropdown;

	private void OnEnable() { //TODO shouldn't it be in Awake() ?
		_dropdown = GetComponent<Dropdown>();

		_dropdown.options = new List<Dropdown.OptionData>();
		foreach (VideoDisplay vd in VideosDisplayer.Instance.Displays) {
			if (vd != VideoDisplay.expandedDisplay || !_ignoredCurrentlyExpandedDisplay) {
				_dropdown.options.Add(new Dropdown.OptionData(vd.VideoName));
			}
		}
	}

	public string Selected {
		get {
			return _dropdown.options[_dropdown.value].text;
		}
		set {
			SetSelected(value);
		}
	}
	
	private void SetSelected(string videoName) {
		for (int index = 0; index < _dropdown.options.Count; ++index) {
			if (_dropdown.options[index].text == videoName) {
				_dropdown.value = index;
				_dropdown.captionText.text = videoName;
				return;
			}
		}
		_dropdown.value = 0;
		if (_dropdown.options.Count <= 0) _dropdown.captionText.text = "[???]";
		else _dropdown.captionText.text = _dropdown.options[0].text;
	}
}
