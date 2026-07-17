using System;
using System.IO;
using UnityEngine;
using CI.HttpClient;
using TMPro;
using UnityEngine.UI;

public enum ContentMode
{
    User,
    Guide
}

public class SyncManager : MonoBehaviour
{
    [SerializeField] private ContentMode contentMode;
    [SerializeField] private TMP_InputField _IPAdressInputField;
    [SerializeField] private Text _progressText;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Manifest _manifest;
    [SerializeField] private string _baseUrl;
    
    private HttpClient _client;  
    private ManifestFile[] _files = Array.Empty<ManifestFile>(); 
    private int _currentFileIndex;  
    private FileStream _fileStream;  
    
    private void Start()
    {
        _client = new HttpClient();  
        _IPAdressInputField.text = PlayerPrefs.GetString("ipAddress", "127.0.0.1:8080");
        OnIpAddressEntered(_IPAdressInputField.text);
    }
    
    public void DownloadManifest() //download the manifest and then download the files corresponding to our setup
    {
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
    
    public void OnIpAddressEntered(string value) 
    { 
        PlayerPrefs.SetString("ipAddress", value);
        _baseUrl = "http://" + value + "/Deploy";
    }

    private void DownloadNextFile()
    {
        _files = contentMode == ContentMode.User ? _manifest.userFiles : _manifest.guideFiles;
        
        if (_currentFileIndex >= _files.Length)
        {
            _progressText.text = "Sync complete";
            Debug.Log("SYNC COMPLETE");  
            return;  
        }  
        
        string localPath = FileUtil.GetLocalPath(_files[_currentFileIndex].filename);  
        
        if (FileUtil.ExistsAndMatches(_files[_currentFileIndex]))  //check if we already downloaded that file!
        {
            DownloadSucceeded();
            return;  
        }  
        
        Debug.Log("Downloading: " + _files[_currentFileIndex].filename);  
        
        DownloadFile(_baseUrl + "/Content/" + contentMode + "/" + _files[_currentFileIndex].filename, localPath, () =>  
        {  
            OnFileDownloaded(_files[_currentFileIndex], localPath);  
        }, (error) =>  
        {  
            Debug.LogError("Download error: " + error);  
        }, null);  
    }
    
    private void OnFileDownloaded(ManifestFile file, string localPath)  
    {  
        string localHash = HashUtil.Sha256(localPath);  
  
        if (localHash != file.sha256)  
        {  
            Debug.LogError($"Hash mismatch for {file.filename}");
            File.Delete(localPath);
            
            Debug.Log($"Retrying download for {file.filename}...");
            DownloadNextFile();
        }  
        else  
        {  
            Debug.Log("downloaded and checksumed " + file.filename);
            DownloadSucceeded();
        }  
    }  
    
    private void DownloadFile(string url, string localPath, Action onComplete, Action<string> onError, Action<int> onProgress) 
    {  
        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
        var tempFilePath = localPath + ".tmp"; //write to temp file so that a failed download never leaves a partially written file
        if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
          
        _client.Get(new Uri(url), HttpCompletionOption.StreamResponseContent, (r) =>  
        {  
            if (r.IsSuccessStatusCode && _fileStream == null)
                _fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);    // Create the output stream once the download begins successfully.
              
            if (r.ContentReadThisRound > 0 && _fileStream != null)  // Write any received chunk to disk.
            {  
                byte[] data = r.ReadAsByteArray();  
                _fileStream.Write(data, 0, data.Length);  
                onProgress?.Invoke(r.PercentageComplete);  
            }  
  
            if (r.PercentageComplete == 100 || !r.IsSuccessStatusCode)  
            {  
                _fileStream?.Flush();
                _fileStream?.Close();  
                _fileStream = null;  

                if (r.IsSuccessStatusCode)
                {
                    if (File.Exists(localPath)) File.Delete(localPath); // ATOMIC REPLACE STEP
                    File.Move(tempFilePath, localPath);
                    onComplete?.Invoke();  
                }  
                else  
                {
                    if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                    _progressText.text = "Error downloading file " + tempFilePath;
                    onError?.Invoke("Download failed: " + r.StatusCode);  
                }
            }
            
            _progressText.text = "Downloading file " + _currentFileIndex + " of " +  _files.Length +  ": " + r.PercentageComplete + "%";  
            _progressSlider.value = r.PercentageComplete;  
        });  
    }  
    
    private void DownloadSucceeded()
    {
        _currentFileIndex++;
        DownloadNextFile();
    }
    
}