using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private Outline _outline;
    [SerializeField] private Dropdown _dropdown;

    public string selectedDifficulty = "Normal";
    
    public void EditMode(bool edit)
    {
        _outline.enabled = edit;
        _dropdown.enabled = edit;
    }

    public void DropdownOptionSelected(int option)
    {
        if (option == 1) selectedDifficulty = "Easy";
        else if (option == 2) selectedDifficulty = "Normal";
        else if (option == 3) selectedDifficulty = "Hard";
    }
}
