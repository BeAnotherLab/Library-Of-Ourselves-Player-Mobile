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
    
    private ManifestFile[] _files = Array.Empty<ManifestFile>(); 
    private int _currentFileIndex;  
    private FileStream _fileStream;  

    private void Start()
    {
        _IPAdressInputField.text = PlayerPrefs.GetString("ipAddress", "127.0.0.1:8080");
        OnIpAddressEntered(_IPAdressInputField.text);
    }
    
    public void StartDownload()
    {
        DownloadManifest(OnManifestLoaded, OnManifestError);  
    }
    
    public void OnIpAddressEntered(string value) 
    { 
        PlayerPrefs.SetString("ipAddress", value);
        _baseUrl = "http://" + value + "/Deploy";
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
        
        switch (contentMode)
        {
            case ContentMode.User:
                _files = _manifest.userFiles;  
                break;
            
            case ContentMode.Guide:
                _files = _manifest.guideFiles;  
                break;
        }
        
        if (_currentFileIndex >= _files.Length)
        {
            _progressText.text = "Sync complete";
            Debug.Log("SYNC COMPLETE");  
            return;  
        }  
        
        var file = _files[_currentFileIndex];  
        string localPath = FileUtil.GetLocalPath(file.filename);  
        
        if (FileUtil.ExistsAndMatches(file))  //check if we already downloaded that file!
        {  
            _currentFileIndex++;  
            DownloadNextFile();  
            return;  
        }  
        
        Debug.Log("Downloading: " + file.filename);  
        string url = _baseUrl + "/Content/" + contentMode + "/" + file.filename;  
        
        DownloadFile(url, localPath, () =>  
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
            Debug.LogError("Hash mismatch, retrying: " + file.filename);  //TODO are we actually retrying
            File.Delete(localPath);  
            string url = _baseUrl + file.filename;  
              
            DownloadFile(url, localPath, () =>  
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
            Debug.Log("downloaded and checksumed " + file.filename );
            _currentFileIndex++;  
            DownloadNextFile();  
        }  
    }  
    
    private void DownloadFile(string url, string localPath, System.Action onComplete, System.Action<string> onError, System.Action<int> onProgress) 
    {  
        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
        var tempFilePath = localPath + ".tmp"; //write to temp file so that a failed download never leaves a partially written file
        if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
        HttpClient client = new HttpClient();  
          
        client.Get(new System.Uri(url), HttpCompletionOption.StreamResponseContent, (r) =>  
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
            
            // Update UI  
            _progressText.text = "Downloading file " + _currentFileIndex + " of " +  _files.Length +  ": " + r.PercentageComplete + "%";  
            _progressSlider.value = r.PercentageComplete;  
        });  
    }  
    
    private void DownloadManifest(Action<Manifest> onComplete, Action<string> onError)  
    {  
        HttpClient client = new HttpClient();  
        string url = _baseUrl + "/manifest.json";  
  
        client.Get(new Uri(url), HttpCompletionOption.AllResponseContent, (r) =>  
        {  
            if (!r.IsSuccessStatusCode)  
            {  
                onError?.Invoke("Download failed: " + r.StatusCode);  
                return;  
            }  
            try  
            {  
                _manifest = r.ReadAsJson<Manifest>();  
                onComplete?.Invoke(_manifest);  
            }  
            catch (Exception e)  
            {  
                onError?.Invoke("JSON parse error: " + e.Message);  
            }  
        });  
    }
    
}