using System;
using UnityEngine;
using System.Collections;
using System.IO;
using RenderHeads.Media.AVProVideo;
using SimpleFileBrowser;

public class FileBrowsing : MonoBehaviour
{
	public delegate void OnRootFolderPicked();
	public static OnRootFolderPicked RootFolderPicked;

	private void OnEnable()
	{
		Settings.PickFolderButtonPresed += ShowFileBrowsingDialog;
	}

	private void OnDisable()
	{
		Settings.PickFolderButtonPresed -= ShowFileBrowsingDialog;
	}

	void Start()
	{
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null ); //TODO remove?
		
		//show file browse on startup if no path has been picked yet
		if (PlayerPrefs.GetString("GuidePath", "") == "") ShowFileBrowsingDialog(); // TODO show a dialog before opening browser?
	}

	private void ShowFileBrowsingDialog()
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		FileBrowserHelpers.AJC.CallStatic( "PickSAFFolder", FileBrowserHelpers.Context, new FBDirectoryReceiveCallbackAndroid( OnSAFDirectoryPicked ) );
#endif
	}
	
	private void OnFilesSelected(string filePath)
	{
		PlayerPrefs.SetString("GuidePath", filePath);
		DataFolder.GuidePath = filePath;
		RootFolderPicked();
	}
	
	private void OnSAFDirectoryPicked(string rawUri, string name)
	{
       if (rawUri != "") OnFilesSelected(rawUri); // FileBrowser.Result is null, if FileBrowser.Success is false
	}
}