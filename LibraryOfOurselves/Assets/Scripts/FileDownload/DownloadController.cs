using System.IO;
using CI.HttpClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownloadController : MonoBehaviour
{
    public TMP_InputField ipAdress;
    public Text downloadProgress;
    public Slider ProgressSlider;


    public void Download()  
    {  
        HttpClient client = new HttpClient();  
  
        ProgressSlider.value = 100;  
  
        client.Get(new System.Uri("http://" + ipAdress.text + " /Content/"), HttpCompletionOption.AllResponseContent, (r) =>  
        {  
            // Save to persistent data path  
            string filePath = Path.Combine(Application.persistentDataPath, "downloaded_data.bin"); // Save to persistent data path  
            File.WriteAllBytes(filePath, r.ReadAsByteArray());  
          
            // Update UI  
            downloadProgress.text = "Download: " + r.PercentageComplete.ToString() + "%";  
            ProgressSlider.value = 100 - r.PercentageComplete;  
        });  
    }

}