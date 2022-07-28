using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour
{
   public InputField choiceInputField;
   public VideoNamesDropdown optionDropdown;

   public delegate void OnEditButtonClicked();
   public static OnEditButtonClicked EditButtonClicked; 
   
   public void OnClickDeleteButton()
   {
      Destroy(gameObject);
   }

   public void OnClickEditButton()
   {
      EditButtonClicked.Invoke();
   }
}
