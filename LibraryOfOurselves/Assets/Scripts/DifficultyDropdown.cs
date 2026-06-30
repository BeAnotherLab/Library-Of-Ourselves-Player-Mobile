using System;
using System.Collections;
using System.Collections.Generic;
using Haze;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty { Easy, Normal, Hard };

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private Outline _outline;
    [SerializeField] private Dropdown _dropdown;
    [SerializeField] private GameObject[] _translatedGameObjectsNames; //Easy, Normal, Hard
    [SerializeField] private Text _difficultyText;
    
    public Difficulty selectedDifficulty;
    
    public void EditMode(bool edit)
    {
        _arrow.enabled = edit;
        _outline.enabled = edit;
        _dropdown.enabled = edit;
        if (edit)
        {
            for (int i = 0; i < 3; i++) _dropdown.options[i].text = _translatedGameObjectsNames[i].name;
        }
    }

    public void DropdownOptionSelected(int option)
    {
        _difficultyText.text = _translatedGameObjectsNames[option].name;
        if (option == 0) selectedDifficulty = Difficulty.Easy;
        else if (option == 1) selectedDifficulty = Difficulty.Normal;
        else if (option == 2) selectedDifficulty = Difficulty.Hard;
    }

    public void SetDifficultyValue(int index) //used to set the value that should be displayed in the interface
    { 
        _dropdown.SetValueWithoutNotify(index);
    }
}
