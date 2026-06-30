using CI.HttpClient;  
using UnityEngine;  
  
public class ManifestLoader : MonoBehaviour  
{  
    public Manifest Manifest;  
  
    public void DownloadManifest(System.Action<Manifest> onComplete, System.Action<string> onError)  
    {  
        HttpClient client = new HttpClient();  
        string url = DeployConfig.BaseUrl + "/manifest.json";  
  
        client.Get(new System.Uri(url), HttpCompletionOption.AllResponseContent, (r) =>  
        {  
            if (!r.IsSuccessStatusCode)  
            {  
                onError?.Invoke("Download failed: " + r.StatusCode);  
                return;  
            }  
  
            try  
            {  
                Manifest = r.ReadAsJson<Manifest>();  
                onComplete?.Invoke(Manifest);  
            }  
            catch (System.Exception e)  
            {  
                onError?.Invoke("JSON parse error: " + e.Message);  
            }  
        });  
    }  
}