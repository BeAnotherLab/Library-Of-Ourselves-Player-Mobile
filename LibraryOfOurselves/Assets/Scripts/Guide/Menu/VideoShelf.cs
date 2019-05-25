using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VideoShelf : MonoBehaviour {

	[SerializeField] Text titleDisplay;
	[SerializeField] Image thumbnailDisplay;
	[SerializeField] Text descriptionDisplay;
	[SerializeField] Text objectsDisplay;
	[SerializeField] Text is360Display;
	[SerializeField] Text editDisplay;
	[SerializeField] Text saveDisplay;
	[SerializeField] InputField descriptionInputField;
	[SerializeField] InputField objectsInputField;
	[SerializeField] Toggle is360Toggle;
	[SerializeField] GameObject noDescriptionTranslation;
	[SerializeField] GameObject noObjectNeededTranslation;

	VideoDisplay current = null;

	bool __editMode = true;
	bool enableSave = false;
	bool EditMode {
		get { return __editMode; }
		set {
			if(__editMode != value) {
				__editMode = value;

				if(value) {//go into Edit Mode
					editDisplay.gameObject.SetActive(false);
					saveDisplay.gameObject.SetActive(true);

					descriptionInputField.gameObject.SetActive(true);
					objectsInputField.gameObject.SetActive(true);
					descriptionInputField.text = current.Settings.description;
					objectsInputField.text = current.Settings.objectsNeeded;

					is360Toggle.gameObject.SetActive(true);
					is360Toggle.isOn = current.Settings.is360;
					is360Display.gameObject.SetActive(false);

				} else {//save and quit Edit Mode
					editDisplay.gameObject.SetActive(true);
					saveDisplay.gameObject.SetActive(false);
					descriptionInputField.gameObject.SetActive(false);
					objectsInputField.gameObject.SetActive(false);
					is360Toggle.gameObject.SetActive(false);
					is360Display.gameObject.SetActive(true);

					if(enableSave) {
						current.Settings.description = descriptionInputField.text;
						current.Settings.objectsNeeded = objectsInputField.text;
						current.Settings.is360 = is360Toggle.isOn;

						//and save them!
						VideosDisplayer.Instance.SaveVideoSettings(current.FullPath, current.VideoName, current.Settings);

						//update display...
						DisplayCurrentVideo();
					}

				}

			}
		}
	}

	public void DisplayCurrentVideo() {

		current = VideoDisplay.expandedDisplay;

		titleDisplay.text = current.VideoName;
		thumbnailDisplay.sprite = current.Thumbnail;
		if(current.Settings.description != "")
			descriptionDisplay.text = current.Settings.description;
		else
			descriptionDisplay.text = noDescriptionTranslation.name;
		if(current.Settings.objectsNeeded != "")
			objectsDisplay.text = current.Settings.objectsNeeded;
		else
			objectsDisplay.text = noObjectNeededTranslation.name;
		is360Display.gameObject.SetActive(current.Settings.is360);

		enableSave = false;
		EditMode = false;
		enableSave = true;

	}

	public void OnClickChoose() {
		if(current) {
			current.OnClickChoose();
		}
	}

	public void OnClickEdit() {
		EditMode = !EditMode;
	}

}
