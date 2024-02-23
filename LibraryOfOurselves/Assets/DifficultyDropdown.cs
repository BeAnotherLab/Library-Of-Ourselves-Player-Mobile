using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty { Easy, Normal, Hard };

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private Outline _outline;
    [SerializeField] private Dropdown _dropdown;

    public Difficulty selectedDifficulty;
    
    public void EditMode(bool edit)
    {
        _arrow.enabled = edit;
        _outline.enabled = edit;
        _dropdown.enabled = edit;
    }

    public void DropdownOptionSelected(int option)
    {
        if (option == 0) selectedDifficulty = Difficulty.Easy;
        else if (option == 1) selectedDifficulty = Difficulty.Normal;
        else if (option == 2) selectedDifficulty = Difficulty.Hard;
    }

    public void SetDifficultyValue(int index) //used to set the value that should be displayed in the interface
    { 
        _dropdown.SetValueWithoutNotify(index);
    }
}
