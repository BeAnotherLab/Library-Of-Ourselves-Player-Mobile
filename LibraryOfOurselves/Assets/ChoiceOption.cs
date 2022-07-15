using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour
{
   [SerializeField] private InputField _choiceInputField;
   [SerializeField] private Dropdown _optionDropdown;
   [SerializeField] private Button _deleteSelf;


   public void OnClickDeleteButton()
   {
      Destroy(gameObject);
   }
   
   
}
