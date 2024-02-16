using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngineInternal.Input;

public class VideoShelf : MonoBehaviour { //displays a single video, along with choice and orientation editors

	//TODO use gameobject when sufficient
	[SerializeField] private Text _titleDisplay;
	[SerializeField] private Image _thumbnailDisplay;
	[SerializeField] private Text _descriptionDisplay;
	[SerializeField] private Text _objectsDisplay;
	[SerializeField] private GameObject _is360Display;
	[SerializeField] private Text _editDisplay; 
	[SerializeField] private Text _saveDisplay;
	[SerializeField] private InputField _descriptionInputField;
	[SerializeField] private InputField _objectsInputField;
	[SerializeField] private Toggle _is360Toggle;
	[SerializeField] private GameObject _noDescriptionTranslation;
	[SerializeField] private GameObject _noObjectNeededTranslation;
	[SerializeField] private Button _editButton;
	[SerializeField] private Button _chooseButton;
	[SerializeField] private string _appendToObjects = "";
	[SerializeField] private bool _saveAsOldSettings = false;

	[Header("Choice editor")]
	[SerializeField] private Text _editChoice;
	[SerializeField] private GameObject _addChoiceTranslation;
	[SerializeField] private GameObject _choiceEditionPanel;
	[SerializeField] private Transform _optionFieldsParent;
	[SerializeField] private GameObject _choiceUIPrefab;
	
	VideoDisplay current;

	bool __editMode = true;
	
	private bool EditMode {
		get { return __editMode; }
		set {
			if (__editMode != value) {
				__editMode = value;
				
				_editDisplay.gameObject.SetActive(!value); //hide edit button if ¿not null?
				_saveDisplay.gameObject.SetActive(value); //show save button

				_descriptionInputField.gameObject.SetActive(value); //allow edition of video description
				_objectsInputField.gameObject.SetActive(value); //allow edition of objects to be used
				
				_is360Toggle.gameObject.SetActive(value);
				_editChoice.gameObject.SetActive(value);

				if (value) //go into Edit Mode 
				{
					_descriptionInputField.text = current.Settings.description;
					_objectsInputField.text = current.Settings.objectsNeeded;

					_is360Toggle.isOn = current.Settings.is360;
					
					_is360Display.gameObject.SetActive(!value);
					
					//Change the displayed text to "Add Choice" if there's no choices available yet
					if (current.Settings.choices.Count == 0) _editChoice.text = _addChoiceTranslation.name;
				} 
				else //save and quit Edit Mode 
				{
					_is360Display.gameObject.SetActive(current.Settings.is360);

					current.Settings.description = _descriptionInputField.text;
					current.Settings.objectsNeeded = _objectsInputField.text;
					current.Settings.is360 = _is360Toggle.isOn;

					VideosDisplayer.Instance.SaveVideoSettings(current.VideoName, current.Settings);

					DisplayCurrentVideo(); //update display...
				}
			}
		}
	}

	private void OnDisable() {
		if (VideoDisplay.expandedDisplay != null) VideoDisplay.expandedDisplay.Contract();
	}

	public void DisplayCurrentVideo() {

		current = VideoDisplay.expandedDisplay;

		_titleDisplay.text = current.VideoName;
		_thumbnailDisplay.sprite = current.Thumbnail;
		
		if (current.Settings.description != "") _descriptionDisplay.text = current.Settings.description;
		else _descriptionDisplay.text = _noDescriptionTranslation.name;
		
		if (current.Settings.objectsNeeded != "") _objectsDisplay.text = _appendToObjects + current.Settings.objectsNeeded;
		else _objectsDisplay.text = _noObjectNeededTranslation.name;
			
		if (_is360Display != null) _is360Display.SetActive(current.Settings.is360);

		EditMode = false;
		
		_editButton.gameObject.SetActive(SettingsAuth.TemporalUnlock);

		int currentlyPaired = 0;
		if (ConnectionsDisplayer.Instance != null) 
		{
			foreach (ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) 
				if (handle.connection.active && handle.connection.paired)
					++currentlyPaired;
		}
		else currentlyPaired = 1;//Assume one connection when there are no display. 
		
		if (currentlyPaired > 0 && current.Available) //if there's no connections (or some of them don't have the video), cannot play any videos. 
		{
			_chooseButton.gameObject.SetActive(true);
		} 
		else 
		{
			_chooseButton.gameObject.SetActive(false);
			Haze.Logger.Log("Hiding choose button (currentlyPaired = " + currentlyPaired + "; current.Available = " + current.Available + ")");
		}
	}

	public void OnClickChoose() {
		if (current) current.OnClickChoose();
	}

	public void OnClickEdit() {
		EditMode = !EditMode;
	}

	public void OnClickEditChoice() 
	{
		_choiceEditionPanel.SetActive(true);
		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;
		if (settings.choices.Count > 0) //if there are options
		{
			ClearOptions();
			foreach (VideoChoice choice in settings.choices) AddChoice(choice); //go through options an instantiate their prefab
		}
	}

	public void ClearOptions()
	{
		while (_optionFieldsParent.childCount > 0) DestroyImmediate(_optionFieldsParent.GetChild(0).gameObject);
	}
	
	public void OnClickAddChoice()
	{
		AddChoice(new VideoChoice()); 
	}
	
	private void AddChoice(VideoChoice choice)
	{
		var instance = Instantiate(_choiceUIPrefab, _optionFieldsParent);
		instance.GetComponentInChildren<InputField>().text = choice.description;
		instance.GetComponentInChildren<VideoNamesDropdown>().Selected = choice.video;
		instance.GetComponent<ChoiceOption>().eulerAngles = choice.position;
		var dataPresent = (choice.position.x != 0 || choice.position.y != 0 || choice.position.z != 0);
		instance.GetComponent<ChoiceOption>().SetEditButtonColor(dataPresent);	
		
	}
	
	private void OnClickSaveChoice() {
		_choiceEditionPanel.SetActive(false);

		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;

		settings.choices = new List<VideoChoice>();
		
		foreach (ChoiceOption choiceOption in _optionFieldsParent.GetComponentsInChildren<ChoiceOption>())
		{
			Debug.Log("under choiceTransform " + choiceOption.gameObject.name);
			var videoChoice = new VideoChoice();
			videoChoice.description = choiceOption.choiceInputField.text;
			videoChoice.video = choiceOption.optionDropdown.Selected;
			videoChoice.position = choiceOption.eulerAngles;
			settings.choices.Add(videoChoice);
		}

		VideoDisplay.expandedDisplay.Settings = settings;

		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.Expand();
	}

	private void OnClickDeleteChoice() {
		_choiceEditionPanel.SetActive(false);
		VideoSettings settings = VideoDisplay.expandedDisplay.Settings;
		VideoDisplay.expandedDisplay.Settings = settings;
		VideosDisplayer.Instance.SaveVideoSettings(VideoDisplay.expandedDisplay.VideoName, settings);
		VideoDisplay.expandedDisplay.Expand();
	}

}
