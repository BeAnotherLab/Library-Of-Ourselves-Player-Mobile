using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class XRHardwareDetector : MonoBehaviour
{
    [SerializeField] bool _allowSceneLoad ;
    
    private void Start()
    { 
        StartCoroutine(WaitAndDisableSceneLoad());
    }

    private void Update()
    {
        // 1. Get the Right Hand Device
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // 2. Check if the device is connected and valid
        if (rightHandDevice.isValid && _allowSceneLoad)
        {
            // 3. Read the feature state directly (e.g., primaryButton = 'A')
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
            {
                Debug.Log("A Button is currently being held down!");
                // Read trigger state as a float (0.0 to 1.0)
                if (rightHandDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.5f)
                {
                    Debug.Log("Right Trigger is pulled!");
                    SceneManager.LoadScene("ContentDownloadVR");
                }
            }
        }
    }

    private IEnumerator WaitAndDisableSceneLoad()
    {
        _allowSceneLoad = true;
        yield return new WaitForSeconds(10);
        _allowSceneLoad = false;
        ;
    }
}