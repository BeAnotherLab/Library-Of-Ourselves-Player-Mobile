using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleToggle : MonoBehaviour
{
    public delegate void OnConsoleToggle(bool on);
    public static OnConsoleToggle ConsoleToggled;

    [SerializeField] private Toggle _consoleToggle;
    
    private void Start()
    {
        _consoleToggle = GetComponent<Toggle>();
        if (PlayerPrefs.GetInt("ToggleOn", 0) == 0) 
            _consoleToggle.SetIsOnWithoutNotify(false);
        else 
            _consoleToggle.SetIsOnWithoutNotify(true);
    }

    public void Toggle(bool on)
    {
        ConsoleToggled(on);
        if (on) PlayerPrefs.SetInt("ToggleOn", 1);
        else PlayerPrefs.SetInt("ToggleOn", 0);
    }
}
