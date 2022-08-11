using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour //Tablet UI script for storing the data of one of the options to the choice asked to the use
{
   public InputField choiceInputField;
   public VideoNamesDropdown optionDropdown;
   public Vector3 eulerAngles;
   
   public delegate void OnEditButtonClicked(string videoName, string description, Vector3 eulerAngles);
   public static OnEditButtonClicked EditButtonClicked;

   [SerializeField] private GameObject _choiceEditButton;
   [SerializeField] private GameObject _choiceSaveButton;
   
   public void OnClickDeleteButton()
   {
      Destroy(gameObject);
   }

   public void OnClickEditChoicePositionButton()
   {
      _choiceEditButton.SetActive(false);
      _choiceSaveButton.SetActive(true);
      EditButtonClicked.Invoke( optionDropdown.Selected, choiceInputField.text, eulerAngles);
   }
   {
      EditButtonClicked.Invoke();
   }
}
