﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngineInternal.Input;

public class VideoShelf : MonoBehaviour { //displays a single video, along with choice and orientation editors

	[SerializeField] Text titleDisplay;
	[SerializeField] Image thumbnailDisplay;
	[SerializeField] Text descriptionDisplay;
	[SerializeField] Text objectsDisplay;
	[SerializeField] GameObject is360Display;
	[SerializeField] Text editDisplay;
	[SerializeField] Text saveDisplay;
	[SerializeField] InputField descriptionInputField;
	[SerializeField] InputField objectsInputField;
	[SerializeField] Toggle is360Toggle;
	[SerializeField] GameObject noDescriptionTranslation;
	[SerializeField] GameObject noObjectNeededTranslation;
	[SerializeField] Button editButton;
	[SerializeField] Button chooseButton;
	[SerializeField] string appendToObjects = "";
	[SerializeField] bool saveAsOldSettings = false;
	[SerializeField] UnityEvent onClickChoose;
	[SerializeField] UnityEvent onClickEdit;
	[SerializeField] UnityEvent onClickSave;

	[Header("Choice editor")]
	[SerializeField] Text editChoice;
	[SerializeField] GameObject addChoiceTranslation;
	[SerializeField] GameObject choiceEditionPanel;
	[SerializeField] InputField questionField;
	[SerializeField] Transform optionFieldsParent;
	
	[SerializeField] private GameObject choiceUIPrefab;
	
	[Header("Orientation editor")]
	[SerializeField] Text editOrientation;
	[SerializeField] GameObject orientationEditionPanel;
	[SerializeField] OrientationEditor orientationEditor;

	[Header("Old-style orientation editor")]
	[SerializeField] InputField pitchInputField;
	[SerializeField] InputField yawInputField;
	[SerializeField] InputField rollInputField;

	VideoDisplay current;

	bool __editMode = true;
	bool enableSave ;
	
	bool EditMode {
		get { return __editMode; }
		set {
			if (__editMode != value) {
				__editMode = value;

				if (value) //go into Edit Mode 
				{
					if (editDisplay != null) editDisplay.gameObject.SetActive(false); //hide edit button if ¿not null?
					if (saveDisplay != null) saveDisplay.gameObject.SetActive(true); //show save button

					descriptionInputField.gameObject.SetActive(true); //allow edition of video description
					objectsInputField.gameObject.SetActive(true); //allow edition of objects to be used
					descriptionInputField.text = current.Settings.description;
					objectsInputField.text = current.Settings.objectsNeeded;

					if(is360Toggle != null) {
						is360Toggle.gameObject.SetActive(true);
						is360Toggle.isOn = current.Settings.is360;
					}
					
					if (is360Display != null) is360Display.gameObject.SetActive(false);

					//Old style orientation settings:
					if (current.Settings.deltaAngles.Length > 0) {
						Vector4 deltas = current.Settings.deltaAngles[0];
						if(pitchInputField) pitchInputField.SetTextWithoutNotify(""+deltas.x);
						if(yawInputField) yawInputField.SetTextWithoutNotify("" + deltas.y);
						if(rollInputField) rollInputField.SetTextWithoutNotify("" + deltas.z);
					}

					if (editChoice != null) {
						editChoice.gameObject.SetActive(true);
						//Change the displayed text to "Add Choice" if there's no choices available yet
						if (current.Settings.choices.Count == 0) editChoice.text = addChoiceTranslation.name;
					}

					if (editOrientation != null) editOrientation.gameObject.SetActive(true);

				} 
				else //save and quit Edit Mode 
				{
					if (editDisplay != null) editDisplay.gameObject.SetActive(true);
					if (saveDisplay != null) saveDisplay.gameObject.SetActive(false);
					
					descriptionInputField.gameObject.SetActive(false);
					objectsInputField.gameObject.SetActive(false);
					
					if (is360Toggle != null) is360Toggle.gameObject.SetActive(false);
					if (is360Display != null) is360Display.gameObject.SetActive(current.Settings.is360);

					if (editChoice != null) editChoice.gameObject.SetActive(false);
					if (editOrientation != null) editOrientation.gameObject.SetActive(false);

					if (enableSave) 
					{
						current.Settings.description = descriptionInputField.text;
						current.Settings.objectsNeeded = objectsInputField.text;
						current.Settings.is360 = is360Toggle.isOn;

						//Old-style orientation save
						if(pitchInputField && yawInputField && rollInputField) {
							Vector4 deltas = Vector4.zero;
							float.TryParse(pitchInputField.text, out deltas.x);
							float.TryParse(yawInputField.text, out deltas.y);
							float.TryParse(rollInputField.text, out deltas.z);
							current.Settings.deltaAngles = new Vector4[] { deltas };
						}

						VideosDisplayer.Instance.SaveVideoSettings(current.FullPath, current.VideoName, current.Settings);

						//update display...
						DisplayCurrentVideo();
					}
				}
			}
		}
	}

	private void OnDisable() {
		if (VideoDisplay.expandedDisplay != null) {
			VideoDisplay.expandedDisplay.contract();
		}
	}

	public void DisplayCurrentVideo() {

		current = VideoDisplay.expandedDisplay;

		titleDisplay.text = current.VideoName;
		thumbnailDisplay.sprite = current.Thumbnail;
		
		if (current.Settings.description != "") descriptionDisplay.text = current.Settings.description;
		else descriptionDisplay.text = noDescriptionTranslation.name;
		
		if (current.Settings.objectsNeeded != "") objectsDisplay.text = appendToObjects + current.Settings.objectsNeeded;
		else objectsDisplay.text = noObjectNeededTranslation.name;
			
		if (is360Display != null) is360Display.SetActive(current.Settings.is360);

		enableSave = false;
		EditMode = false;
		enableSave = true;

		editButton.gameObject.SetActive(SettingsAuth.TemporalUnlock);

		int currentlyPaired = 0;
		if (ConnectionsDisplayer.Instance != null) 
		{
			foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) 
				if (handle.connection.active && handle.connection.paired)
					++currentlyPaired;
		}
		else currentlyPaired = 1;//Assume one connection when there are no display. 
		
		if (currentlyPaired > 0 && current.Available) //if there's no connections (or some of them don't have the video), cannot play any videos. 
		{
			chooseButton.gameObject.SetActive(true);
		} 
		else 
		{
			chooseButton.gameObject.SetActive(false);
			Haze.Logger.Log("Hiding choose button (currentlyPaired = " + currentlyPaired + "; current.Available = " + current.Available + ")");
		}
	}

	public void OnClickChoose() {
		if(current) {
			current.OnClickChoose();
			onClickChoose.Invoke();
		}
	}

	public void OnClickEdit() {
		EditMode = !EditMode;
		if(EditMode)
			onClickEdit.Invoke();
		else
			onClickSave.Invoke();
	}

	public void OnClickEditChoice() 
	{
		choiceEditionPanel.SetActive(true);
		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;
		if (settings.choices.Count > 0) //if there are options
		{
			ClearOptions();
			foreach (VideoChoice choice in settings.choices) AddChoice(choice); //go through options an instantiate their prefab
		}
	}

	public void ClearOptions()
	{
		while (optionFieldsParent.childCount > 0) DestroyImmediate(optionFieldsParent.GetChild(0).gameObject);
	}
	
	public void OnClickAddChoice()
	{
		AddChoice(new VideoChoice()); 
	}
	
	public void OnClickEditOrientation() {
		orientationEditionPanel.SetActive(true);

		Vector4[] deltas = VideoDisplay.expandedDisplay.Settings.deltaAngles;
		if(deltas.Length == 0)
			orientationEditor.ResetOrientations(true);
		else {
			orientationEditor.ResetOrientations(false);
			foreach(Vector4 delta in deltas) {
				orientationEditor.AddOrientation(delta);
			}
		}
	}

	public void OnClickSaveOrientation() {
		orientationEditionPanel.SetActive(false);

		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;

		settings.deltaAngles = orientationEditor.GetValues().ToArray();

		VideoDisplay.expandedDisplay.Settings = settings;

		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.FullPath, VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.expand();
	}
	
	private GameObject AddChoice(VideoChoice choice)
	{
		var instance = Instantiate(choiceUIPrefab, optionFieldsParent);
		instance.GetComponentInChildren<InputField>().text = choice.description;
		instance.GetComponentInChildren<VideoNamesDropdown>().Selected = choice.video;
		instance.GetComponent<ChoiceOption>().eulerAngles = choice.position;
		return instance;
	}
	
	private void OnClickSaveChoice() {
		choiceEditionPanel.SetActive(false);

		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;

		settings.choices = new List<VideoChoice>();
		
		foreach (ChoiceOption choiceOption in optionFieldsParent.GetComponentsInChildren<ChoiceOption>())
		{
			Debug.Log("under choiceTransform " + choiceOption.gameObject.name);
			var videoChoice = new VideoChoice();
			videoChoice.description = choiceOption.choiceInputField.text;
			videoChoice.video = choiceOption.optionDropdown.Selected;
			videoChoice.position = choiceOption.eulerAngles;
			settings.choices.Add(videoChoice);
		}

		VideoDisplay.expandedDisplay.Settings = settings;

		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.FullPath, VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.expand();
	}

	private void OnClickDeleteChoice() {
		choiceEditionPanel.SetActive(false);

		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;
/*
		if(settings.choices.Count > 0) {
			settings.choices = new VideosDisplayer.VideoChoice[0];
		}
*/
		VideoDisplay.expandedDisplay.Settings = settings;

		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.FullPath, VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.expand();
	}

}
