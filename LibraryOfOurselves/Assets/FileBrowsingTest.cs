using System;
using UnityEngine;
using System.Collections;
using System.IO;
using RenderHeads.Media.AVProVideo;
using SimpleFileBrowser;

public class FileBrowserTest : MonoBehaviour
{
	public delegate void OnRootFolderPicked();
	public static OnRootFolderPicked RootFolderPicked;
	
	void Start()
	{
		// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
		// It is sufficient to add a quick link just once
		// Name: Users
		// Path: C:\Users
		// Icon: default (folder icon)
		SimpleFileBrowser.FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
		
		 StartCoroutine( ShowLoadDialogCoroutine() );
	}

	IEnumerator ShowLoadDialogCoroutine()
	{
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Allow multiple selection: true
		// Initial path: default (Documents), Initial filename: empty
		// Title: "Load File", Submit button text: "Load"
		yield return SimpleFileBrowser.FileBrowser.WaitForLoadDialog( SimpleFileBrowser.FileBrowser.PickMode.Folders, true, null, null, "Select Files", "Load" );

		// Dialog is closed
		// Print whether the user has selected some files or cancelled the operation (FileBrowser.Success)
		Debug.Log( SimpleFileBrowser.FileBrowser.Success );

		if( SimpleFileBrowser.FileBrowser.Success )
			OnFilesSelected( SimpleFileBrowser.FileBrowser.Result ); // FileBrowser.Result is null, if FileBrowser.Success is false
	}
	
	void OnFilesSelected( string[] filePaths )
	{
		DataFolder.GuidePath = filePaths[0];
		RootFolderPicked();
	}
}