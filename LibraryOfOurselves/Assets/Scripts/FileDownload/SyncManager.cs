using System.IO;
using UnityEngine;
using CI.HttpClient;
using TMPro;
using UnityEngine.UI;

public class SyncManager : MonoBehaviour  
{  
    [SerializeField] private TMP_InputField _IPAdressInputField;
    [SerializeField] private Text _progressText;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Manifest _manifest;
    [SerializeField] private string _baseUrl;
    
    private int _currentFileIndex = 0;  
    private FileStream _fileStream;  
    private string _tempFilePath;

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
        string url = _baseUrl + "/" + file.path;  
  
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
            Debug.LogError("Hash mismatch, retrying: " + file.path);  
            File.Delete(localPath);  
            string url = _baseUrl + file.path;  
              
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
            Debug.Log("downloaded " + file.path );
            _currentFileIndex++;  
            DownloadNextFile();  
        }  
    }  
    
    private void DownloadFile(string url, string localPath, System.Action onComplete, System.Action<string> onError, System.Action<int> onProgress) 
    {  
        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
        
        _tempFilePath = localPath + ".tmp"; //write to temp file first
        
        if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);
        HttpClient client = new HttpClient();  
          
        client.Get(new System.Uri(url), HttpCompletionOption.StreamResponseContent, (r) =>  
        {  
            if (r.IsSuccessStatusCode && _fileStream == null)  
            {  
                _fileStream = new FileStream( // OPEN TEMP FILE instead of final file
                    _tempFilePath,
                    FileMode.Create,
                    FileAccess.Write
                );  
            }  
              
            if (r.ContentReadThisRound > 0 && _fileStream != null)  
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
                    // 🔥 ATOMIC REPLACE STEP
                    if (File.Exists(localPath)) File.Delete(localPath);

                    File.Move(_tempFilePath, localPath);

                    onComplete?.Invoke();  
                }  
                else  
                {
                    if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);

                    onError?.Invoke("Download failed: " + r.StatusCode);  
                }
            }
            
            // Update UI  
            _progressText.text = "Download: " + r.PercentageComplete.ToString() + "%";  
            _progressSlider.value = 100 - r.PercentageComplete;  
        });  
    }  
    
    private void DownloadManifest(System.Action<Manifest> onComplete, System.Action<string> onError)  
    {  
        HttpClient client = new HttpClient();  
        string url = _baseUrl + "/manifest.json";  
  
        client.Get(new System.Uri(url), HttpCompletionOption.AllResponseContent, (r) =>  
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
            catch (System.Exception e)  
            {  
                onError?.Invoke("JSON parse error: " + e.Message);  
            }  
        });  
    }
    
}