using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngineInternal.Input;

public class VideoShelf : MonoBehaviour { //displays a single video, along with choice and orientation editors

	//TODO use gameobject when sufficient
	//TODO reorder variables in accordance with display order
	[SerializeField] private Text _titleDisplay;
	[SerializeField] private Image _thumbnailDisplay;
	[SerializeField] private Text _descriptionDisplay;
	[SerializeField] private Text _objectsDisplay;
	[SerializeField] private GameObject _is360Display;
	[SerializeField] private DifficultyDropdown _difficultyDropdown;
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

	[Header("Choice editor")] //TODO move to own script
	[SerializeField] private Text _editChoice;
	[SerializeField] private GameObject _addChoiceTranslation;
	[SerializeField] private GameObject _choiceEditionPanel;
	[SerializeField] private Transform _optionFieldsParent;
	[SerializeField] private GameObject _choiceUIPrefab;
	
	VideoDisplay current;

	private bool _editMode;
	
	private void OnDisable() 
	{
		if (VideoDisplay.expandedDisplay != null) VideoDisplay.expandedDisplay.Contract();
	}

	public void DisplayCurrentVideo() 
	{
		current = VideoDisplay.expandedDisplay;

		_titleDisplay.text = current.VideoName;
		_thumbnailDisplay.sprite = current.Thumbnail;
		
		if (current.Settings.description != "") _descriptionDisplay.text = current.Settings.description;
		else _descriptionDisplay.text = _noDescriptionTranslation.name;
		
		if (current.Settings.objectsNeeded != "") _objectsDisplay.text = _appendToObjects + current.Settings.objectsNeeded;
		else _objectsDisplay.text = _noObjectNeededTranslation.name;
			
		if (_is360Display != null) _is360Display.SetActive(current.Settings.is360);

		if (current.Settings.difficulty == "Easy") _difficultyDropdown.SetDifficultyValue(0);
		else if (current.Settings.difficulty == "Normal") _difficultyDropdown.SetDifficultyValue(1);
		else if (current.Settings.difficulty == "Hard") _difficultyDropdown.SetDifficultyValue(2);
		else _difficultyDropdown.SetDifficultyValue(1);
		
		_editButton.gameObject.SetActive(SettingsAuth.TemporalUnlock);

		int currentlyPaired = 0;
		if (ConnectionsDisplayer.Instance != null) 
		{
			foreach (ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) 
				if (handle.connection.active && handle.connection.paired) ++currentlyPaired;
		}
		else currentlyPaired = 1;//Assume one connection when there are no display. //TODO uh?
		
		if (currentlyPaired > 0 && current.Available) _chooseButton.gameObject.SetActive(true); //if there's no connections (or some of them don't have the video), cannot play any videos. 
		else _chooseButton.gameObject.SetActive(false);
	}

	public void OnClickChoose() 
	{
		if (current) current.OnClickChoose();
	}

	public void ToggleEdit() 
	{
		SetEditMode(!_editMode);
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
	
	private void SetEditMode(bool edit)
	{
		_editMode = edit;
		
		_editDisplay.gameObject.SetActive(!edit); //hide edit button if ¿not null?
		Debug.Log("setting save display visible to " + edit);
		_saveDisplay.gameObject.SetActive(edit); //show save button

		_descriptionInputField.gameObject.SetActive(edit); //allow edition of video description
		_objectsInputField.gameObject.SetActive(edit); //allow edition of objects to be used
		
		_is360Toggle.gameObject.SetActive(edit);
		_editChoice.gameObject.SetActive(edit);

		_difficultyDropdown.EditMode(edit);
		
		if (edit) //go into Edit Mode 
		{
			_descriptionInputField.text = current.Settings.description;
			_objectsInputField.text = current.Settings.objectsNeeded;

			_is360Toggle.isOn = current.Settings.is360;
			
			_is360Display.gameObject.SetActive(!edit);
			
			if (current.Settings.choices.Count == 0) _editChoice.text = _addChoiceTranslation.name; //Change the displayed text to "Add Choice" if there's no choices available yet
		} 
		else //save and quit Edit Mode 
		{
			current.Settings.description = _descriptionInputField.text;
			current.Settings.objectsNeeded = _objectsInputField.text;
			current.Settings.is360 = _is360Toggle.isOn;
			current.Settings.difficulty = _difficultyDropdown.selectedDifficulty.ToString();
			_is360Display.gameObject.SetActive(current.Settings.is360);
	
			VideosDisplayer.Instance.SaveVideoSettings(current.VideoName, current.Settings);

			Debug.Log("exit edit mode");
			///DisplayCurrentVideo(); //update display... //TODO is it necessary?
		}
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
