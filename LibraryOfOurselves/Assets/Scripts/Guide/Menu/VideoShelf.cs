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
	[SerializeField] Button editButton;
	[SerializeField] Button chooseButton;
	[SerializeField] Text editChoice;
	[SerializeField] GameObject addChoiceTranslation;

	[Header("Choice editor")]
	[SerializeField] GameObject choiceEditionPanel;
	[SerializeField] InputField questionField;
	[SerializeField] InputField option1Field;
	[SerializeField] InputField option2Field;
	[SerializeField] VideoNamesDropdown option1Dropdown;
	[SerializeField] VideoNamesDropdown option2Dropdown;

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

					editChoice.gameObject.SetActive(true);
					//Change the displayed text to "Add Choice" if there's no choices available yet
					if(current.Settings.choices.Length == 0) {
						Debug.Log("Settings edit choice to: " + addChoiceTranslation.name);
						editChoice.text = addChoiceTranslation.name;
					}

				} else {//save and quit Edit Mode
					editDisplay.gameObject.SetActive(true);
					saveDisplay.gameObject.SetActive(false);
					descriptionInputField.gameObject.SetActive(false);
					objectsInputField.gameObject.SetActive(false);
					is360Toggle.gameObject.SetActive(false);
					is360Display.gameObject.SetActive(true);

					editChoice.gameObject.SetActive(false);

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

		editButton.gameObject.SetActive(SettingsAuth.TemporalUnlock);

		int currentlyPaired = 0;
		foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
			if(handle.connection.active && handle.connection.paired)
				++currentlyPaired;
		}
		chooseButton.gameObject.SetActive(currentlyPaired > 0);//if there's no connections, cannot play any videos.

	}

	public void OnClickChoose() {
		if(current) {
			current.OnClickChoose();
		}
	}

	public void OnClickEdit() {
		EditMode = !EditMode;
	}

	public void OnClickEditChoice() {
		choiceEditionPanel.SetActive(true);
		VideosDisplayer.VideoSettings settings = VideoDisplay.expandedDisplay.Settings;
		if(settings.choices.Length > 0) {
			questionField.text = settings.choices[0].question;
			option1Field.text = settings.choices[0].option1;
			option2Field.text = settings.choices[0].option2;
			option1Dropdown.Selected = settings.choices[0].video1;
			option2Dropdown.Selected = settings.choices[0].video2;
		} else {
			questionField.text = "";
			option1Field.text = "";
			option2Field.text = "";
			//force default selection:
			option1Dropdown.Selected = "%";
			option2Dropdown.Selected = "%";
		}
	}

	public void OnClickSaveChoice() {
		choiceEditionPanel.SetActive(false);

		VideosDisplayer.VideoSettings settings = VideoDisplay.expandedDisplay.Settings;

		if(settings.choices.Length <= 0) {
			settings.choices = new VideosDisplayer.VideoChoice[1];
			settings.choices[0] = new VideosDisplayer.VideoChoice();
		}

		settings.choices[0].question = questionField.text;
		settings.choices[0].option1 = option1Field.text;
		settings.choices[0].option2 = option2Field.text;
		settings.choices[0].video1 = option1Dropdown.Selected;
		settings.choices[0].video2 = option2Dropdown.Selected;

		VideoDisplay.expandedDisplay.Settings = settings;

		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.FullPath, VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.expand();
	}

	public void OnClickDeleteChoice() {
		choiceEditionPanel.SetActive(false);

		VideosDisplayer.VideoSettings settings = VideoDisplay.expandedDisplay.Settings;

		if(settings.choices.Length > 0) {
			settings.choices = new VideosDisplayer.VideoChoice[0];
		}

		VideoDisplay.expandedDisplay.Settings = settings;

		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.FullPath, VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.expand();
	}

}
