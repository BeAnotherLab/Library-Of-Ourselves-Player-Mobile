using System;
using System.IO;
using UnityEngine;
using CI.HttpClient;
using TMPro;
using UnityEngine.UI;

public enum ContentMode { User, Guide }

public class SyncManager : MonoBehaviour
{
    [SerializeField] private ContentMode contentMode;
    [SerializeField] private TMP_InputField _IPAdressInputField;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Manifest _manifest;
    [SerializeField] private string _baseUrl;
    
    private HttpClient _client;  
    private ManifestFile[] _files = Array.Empty<ManifestFile>(); 
    private int _currentFileIndex;  
    private FileStream _fileStream;
    private bool _isDownloading;
    
    public delegate void OnUpdateDownloadProgressText(string text);
    public static OnUpdateDownloadProgressText UpdateDownloadProgressText;
    
    private void Start()
    {
        _client = new HttpClient();  
        _IPAdressInputField.text = PlayerPrefs.GetString("ipAddress", "127.0.0.1:8080");
        OnIpAddressEntered(_IPAdressInputField.text);
    }
    
    public void DownloadManifest() //download the manifest and then download the files corresponding to our setup
    {
        if (!_isDownloading)
        {
            _isDownloading = true;
            string url = $"{_baseUrl}/manifest.json";

            _client.Get(new Uri(url), HttpCompletionOption.AllResponseContent, r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    Debug.LogError($"Download failed: {r.StatusCode}");
                    return;
                }

                try
                {
                    _manifest = r.ReadAsJson<Manifest>();
                    _currentFileIndex = 0;
                    DownloadNextFile();
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parse error: {e.Message}");
                }
            });
        }
    }
    
    public void OnIpAddressEntered(string value) 
    { 
        PlayerPrefs.SetString("ipAddress", value);
        _baseUrl = "http://" + value + "/Deploy";
    }

    private void DownloadNextFile()
    {
        _files = contentMode == ContentMode.User ? _manifest.userFiles : _manifest.guideFiles;
        var file = _files[_currentFileIndex].filename;
        
        if (_currentFileIndex >= _files.Length) //check if we reached the last file
        {
            UpdateDownloadProgressText("Sync complete");
            Debug.Log("SYNC COMPLETE");  
            _isDownloading = false;
            return;  
        }  
        
        if (FileUtil.ExistsAndMatches(_files[_currentFileIndex])) //if the file is already there, go to the next
        {
            DownloadSucceeded();
            return;  
        }  
        
        Debug.Log("Downloading: " + file);
        
        Directory.CreateDirectory(Path.GetDirectoryName(FileUtil.GetLocalPath(file)));
        var tempFilePath = FileUtil.GetLocalPath(file) + ".tmp"; //write to temp file so that a failed download never leaves a partially written file
        
        if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
          
        _client.Get(new Uri(_baseUrl + "/Content/" + contentMode + "/" + file), HttpCompletionOption.StreamResponseContent, (r) =>  
        {  
            if (r.IsSuccessStatusCode && _fileStream == null)  _fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);    // Create the output stream once the download begins successfully.
               
            if (r.ContentReadThisRound > 0 && _fileStream != null)  // Write any received chunk to disk.
            {  
                byte[] data = r.ReadAsByteArray();  
                _fileStream.Write(data, 0, data.Length);  
            }  
  
            if (r.PercentageComplete == 100 || !r.IsSuccessStatusCode)  
            {  
                _fileStream?.Flush();
                _fileStream?.Close();  
                _fileStream = null;  

                if (r.IsSuccessStatusCode)
                {
                    File.Move(tempFilePath, FileUtil.GetLocalPath(file));
                    //string localHash = HashUtil.Sha256(FileUtil.GetLocalPath(file));  
                    string localHash = FastVideoHash.ComputeHeadTailSha256(FileUtil.GetLocalPath(file)); //TODO test. should run much faster on big files 

                    if (localHash != _files[_currentFileIndex].sha256)  
                    {  
                        Debug.LogError($"Hash mismatch for {file}, retrying download");
                        File.Delete(FileUtil.GetLocalPath(file));
                        DownloadNextFile();
                    }  
                    else  
                    {  
                        Debug.Log("downloaded and checksumed " + file);
                        DownloadSucceeded();
                    }  
                }  
                else  
                {
                    if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                    UpdateDownloadProgressText("Error downloading file " + tempFilePath);
                    Debug.Log("Error downloading file");
                }
            }
            
            UpdateDownloadProgressText("Downloading file " + _currentFileIndex + " of " +  _files.Length +  ": " + r.PercentageComplete + "%");  
            _progressSlider.value = r.PercentageComplete;  
        });  
    }
    
    private void DownloadSucceeded()
    {
        _currentFileIndex++;
        DownloadNextFile();
    }
    
    private void OnDestroy()
    {
        _fileStream?.Close();
        _fileStream?.Dispose();
    }
    
}