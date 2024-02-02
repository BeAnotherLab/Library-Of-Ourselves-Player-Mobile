using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using SimpleFileBrowser;
using UnityEngine;

public class VideoLoader : MonoBehaviour
{
    private MediaPlayer _mediaPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_ANDROID

        _mediaPlayer = GetComponent<MediaPlayer>();
        Debug.Log("can we read from the Movies folder ? ");
        foreach (FileSystemEntry entry in FileBrowserHelpers.GetEntriesInDirectory("/storage/emulated/0/Movies", false))
        {
            Debug.Log(entry);
        }
        
        LoadVideo("/storage/emulated/0/Movies/4KBeach.mp4");
#endif
    }

    public void LoadVideo(string path)
    {
        Debug.Log("Trying to load video  " + path);
        
        if (FileBrowserHelpers.FileExists(path)) Debug.Log("file found");
            
        _mediaPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay:true);
        
    }
}
