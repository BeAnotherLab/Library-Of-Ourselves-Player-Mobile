using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadProgressText : MonoBehaviour
{
    [SerializeField] private Text _progressText;
    
    private void OnEnable()
    {
        SyncManager.UpdateDownloadProgressText += SetText;
    }

    private void OnDisable()
    {
        SyncManager.UpdateDownloadProgressText -= SetText;
    }

    private void SetText(string text)
    {
        _progressText.text = text;
    }
}
