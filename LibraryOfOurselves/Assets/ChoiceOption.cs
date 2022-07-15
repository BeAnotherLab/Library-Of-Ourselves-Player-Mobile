using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour
{
   public InputField choiceInputField;
   public VideoNamesDropdown optionDropdown;
   [SerializeField] private Button _deleteSelf;

   public void OnClickDeleteButton()
   {
      Destroy(gameObject);
   }
}
