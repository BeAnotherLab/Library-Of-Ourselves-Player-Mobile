using CI.HttpClient;
using System.IO;
using UnityEngine;

public class FileDownloader : MonoBehaviour  
{  
    private FileStream _fileStream;  
    private string _tempFilePath;

    public void DownloadFile(string url, string localPath, System.Action onComplete, System.Action<string> onError, System.Action<int> onProgress)  
    {  
        Directory.CreateDirectory(Path.GetDirectoryName(localPath));

        //write to temp file first
        _tempFilePath = localPath + ".tmp";

        if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);

        HttpClient client = new HttpClient();  
          
        client.Get(new System.Uri(url), HttpCompletionOption.StreamResponseContent, (r) =>  
        {  
            if (r.IsSuccessStatusCode && _fileStream == null)  
            {  
                // ✅ OPEN TEMP FILE instead of final file
                _fileStream = new FileStream(
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
        });  
    }  
}