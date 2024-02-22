using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private Outline _outline;
    [SerializeField] private Dropdown _dropdown;
    
    public void EditMode(bool edit)
    {
        _arrow.enabled = edit;
        _outline.enabled = edit;
        _dropdown.enabled = edit;
    }

    public void DifficultyPicked()
    {
        
    }
}
