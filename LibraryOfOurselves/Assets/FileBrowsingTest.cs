using System;
using UnityEngine;
using System.Collections;
using System.IO;
using RenderHeads.Media.AVProVideo;
using SimpleFileBrowser;

public class FileBrowserTest : MonoBehaviour
{
	// Warning: paths returned by FileBrowser dialogs do not contain a trailing '\' character
	// Warning: FileBrowser can only show 1 dialog at a time

	[SerializeField] private MediaPlayer _mediaPlayer;
	
	void Start()
	{
		// Set filters (optional)
		// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
		// if all the dialogs will be using the same filters
		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ), new FileBrowser.Filter( "Text Files", ".txt", ".pdf" ) );

		// Set default filter that is selected when the dialog is shown (optional)
		// Returns true if the default filter is set successfully
		// In this case, set Images filter as the default filter
		FileBrowser.SetDefaultFilter( ".jpg" );

		// Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
		// Note that when you use this function, .lnk and .tmp extensions will no longer be
		// excluded unless you explicitly add them as parameters to the function
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );

		// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
		// It is sufficient to add a quick link just once
		// Name: Users
		// Path: C:\Users
		// Icon: default (folder icon)
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
		
		// !!! Uncomment any of the examples below to show the file browser !!!

		// Example 1: Show a save file dialog using callback approach
		// onSuccess event: not registered (which means this dialog is pretty useless)
		// onCancel event: not registered
		// Save file/folder: file, Allow multiple selection: false
		// Initial path: "C:\", Initial filename: "Screenshot.png"
		// Title: "Save As", Submit button text: "Save"
		// FileBrowser.ShowSaveDialog( null, null, FileBrowser.PickMode.Files, false, "C:\\", "Screenshot.png", "Save As", "Save" );

		// Example 2: Show a select folder dialog using callback approach
		// onSuccess event: print the selected folder's path
		// onCancel event: print "Canceled"
		// Load file/folder: folder, Allow multiple selection: false
		// Initial path: default (Documents), Initial filename: empty
		// Title: "Select Folder", Submit button text: "Select"
		/* FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
								   () => { Debug.Log( "Canceled" ); },
								   FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );
*/
		// Example 3: Show a select file dialog using coroutine approach
		 StartCoroutine( ShowLoadDialogCoroutine() );
	}

	IEnumerator ShowLoadDialogCoroutine()
	{
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Allow multiple selection: true
		// Initial path: default (Documents), Initial filename: empty
		// Title: "Load File", Submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Files, true, null, null, "Select Files", "Load" );

		// Dialog is closed
		// Print whether the user has selected some files or cancelled the operation (FileBrowser.Success)
		Debug.Log( FileBrowser.Success );

		if( FileBrowser.Success )
			OnFilesSelected( FileBrowser.Result ); // FileBrowser.Result is null, if FileBrowser.Success is false
	}
	
	void OnFilesSelected( string[] filePaths )
	{
		// Print paths of the selected files
		for (int i = 0; i < filePaths.Length; i++)
		{
			Debug.Log( filePaths[i] );
			Debug.Log("File exists? " + File.Exists(filePaths[i]));
			
			try
			{
				_mediaPlayer.OpenMedia( new MediaPath( filePaths[i], MediaPathType.AbsolutePathOrURL), autoPlay:true);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}

		// Get the file path of the first selected file
		string filePath = filePaths[0];

		// Read the bytes of the first file via FileBrowserHelpers
		// Contrary to File.ReadAllBytes, this function works on Android 10+, as well
		byte[] bytes = FileBrowserHelpers.ReadBytesFromFile( filePath );

		// Or, copy the first file to persistentDataPath
		string destinationPath = Path.Combine( Application.persistentDataPath, FileBrowserHelpers.GetFilename( filePath ) );
		FileBrowserHelpers.CopyFile( filePath, destinationPath );
	}
}