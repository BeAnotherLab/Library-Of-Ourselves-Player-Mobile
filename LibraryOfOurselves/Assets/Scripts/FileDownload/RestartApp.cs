using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartApp : MonoBehaviour
{
   public void RestartAppButtonClicked()
   {
      if (Application.identifier == "sco.Haze.LibraryOfOurselves")
         SceneManager.LoadScene("VR");   
      else if (Application.identifier == "sco.Haze.LibraryOfOurselvesGuide")
         SceneManager.LoadScene("Guide");   
   }
}
