using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour //Tablet UI script for storing the data of one of the options to the choice asked to the use
{
   public InputField choiceInputField;
   public VideoNamesDropdown optionDropdown;
   public Vector3 eulerAngles;

   public bool selected;
   
   public delegate void OnEditButtonClicked(string videoName, string description, Vector3 eulerAngles);
   public static OnEditButtonClicked EditButtonClicked;

   public delegate void OnSaveButtonClicked(string videoName, string description, Vector3 eulerAngles);
   public static OnSaveButtonClicked SaveButtonClicked;
   
   [SerializeField] private GameObject _choiceEditButton;
   [SerializeField] private GameObject _choiceSaveButton;
   [SerializeField] private Color _noDataColor;
   [SerializeField] private Color _dataColor;

   private void OnEnable()
   {
      EditButtonClicked += EditButtonClickedFromOutside;
      GuideAdapter.ReceivedChoicePosition += OnReceiveChoicePosition;
   }

   public void OnClickDeleteButton()
   {
      Destroy(gameObject);
   }

   private void EditButtonClickedFromOutside(string videoName, string description, Vector3 eulerAngles)
   {
      selected = false; //TODO not sure if this approach for selection is safe
   }
   
   public void OnClickEditChoicePositionButton()
   {
      _choiceEditButton.SetActive(false);
      _choiceSaveButton.SetActive(true);
      EditButtonClicked.Invoke( optionDropdown.Selected, choiceInputField.text, eulerAngles);
      selected = true;
   }

   public void OnClickSaveChoicePositionButton()
   {
      _choiceSaveButton.SetActive(false);
      _choiceEditButton.SetActive(true);
      SaveButtonClicked.Invoke( optionDropdown.Selected, choiceInputField.text, eulerAngles);
   }

   public void OnReceiveChoicePosition(Vector3 angles)
   {
      if(selected) eulerAngles = angles;
   }
}
