using System.IO;
using UnityEngine;

public static class DeployConfig
{
    // Replace with your PC IP
    public static string BaseUrl = "http://192.168.1.69:8080/DeployTest";
}

public class SyncManager : MonoBehaviour  
{  
    public ManifestLoader manifestLoader;  
    public FileDownloader fileDownloader;  
  
    
    private int _currentFileIndex = 0;  
    private Manifest _manifest;  
  
    void Start()  //TODO ENTRY POINT!
    {  
        manifestLoader.DownloadManifest(OnManifestLoaded, OnManifestError);  
    }  
  
    private void OnManifestLoaded(Manifest manifest)  
    {  
        _manifest = manifest;  
        _currentFileIndex = 0;  
        DownloadNextFile();  
    }  
  
    private void OnManifestError(string error)  
    {  
        Debug.LogError("Manifest error: " + error);  
    }  
  
    private void DownloadNextFile()  
    {  
        if (_currentFileIndex >= _manifest.files.Length)  
        {  
            Debug.Log("SYNC COMPLETE");  
            return;  
        }  
  
        var file = _manifest.files[_currentFileIndex];  
        string localPath = FileUtil.GetLocalPath(file.path);  
  
        if (FileUtil.ExistsAndMatches(file))  //check if we already downloaded that file!
        {  
            _currentFileIndex++;  
            DownloadNextFile();  
            return;  
        }  
  
        Debug.Log("Downloading: " + file.path);  
        string url = DeployConfig.BaseUrl + "/" + file.path;  
  
        fileDownloader.DownloadFile(url, localPath, () =>  
        {  
            OnFileDownloaded(file, localPath);  
        }, (error) =>  
        {  
            Debug.LogError("Download error: " + error);  
        }, (progress) =>  
        {  
            // TODO Optional: update progress UI  
        });  
    }  
  
    private void OnFileDownloaded(ManifestFile file, string localPath)  
    {  
        string localHash = HashUtil.Sha256(localPath);  
  
        if (localHash != file.sha256)  
        {  
            Debug.LogError("Hash mismatch, retrying: " + file.path);  
            File.Delete(localPath);  
            string url = DeployConfig.BaseUrl + file.path;  
              
            fileDownloader.DownloadFile(url, localPath, () =>  
            {  
                _currentFileIndex++;  
                DownloadNextFile();  
            }, (error) =>  
            {  
                Debug.LogError("Retry failed: " + error);  
            }, null);  
        }  
        else  
        {  
            Debug.Log("downloaded " + file.path );
            _currentFileIndex++;  
            DownloadNextFile();  
        }  
    }  
}