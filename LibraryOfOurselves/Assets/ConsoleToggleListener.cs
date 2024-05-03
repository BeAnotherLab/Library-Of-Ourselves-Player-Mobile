using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleToggleListener : MonoBehaviour
{

    [SerializeField] private GameObject _consoleGameObject;

    public void OnEnable()
    {
        ConsoleToggle.ConsoleToggled += ToggleConsole;
    }

    public void OnDisable()
    {
        ConsoleToggle.ConsoleToggled -= ToggleConsole;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("ToggleOn", 0) == 0) 
            ToggleConsole(false);
        else 
            ToggleConsole(true);
    }

    private void ToggleConsole(bool on)
    {
        _consoleGameObject.SetActive(on);
    }
}
