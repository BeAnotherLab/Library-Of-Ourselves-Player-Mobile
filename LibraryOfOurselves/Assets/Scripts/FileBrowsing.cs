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
	
	void Start()
	{
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null ); //TODO remove?
		
		//show file browse on startup if no path has been picked yet
		if (PlayerPrefs.GetString("GuidePath", "") == "") StartCoroutine( ShowLoadDialogCoroutine());
	}

	public void ShowFileBrowsingDialog()
	{
		StartCoroutine( ShowLoadDialogCoroutine() );
	}
	
	IEnumerator ShowLoadDialogCoroutine()
	{
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Folders, true, null, null, "Select Files", "Load" );

		if (FileBrowser.Success) OnFilesSelected(FileBrowser.Result); // FileBrowser.Result is null, if FileBrowser.Success is false
	}
	
	private void OnFilesSelected(string[] filePaths)
	{
		PlayerPrefs.SetString("GuidePath", filePaths[0]);
		DataFolder.GuidePath = filePaths[0];
		RootFolderPicked();
	}
}