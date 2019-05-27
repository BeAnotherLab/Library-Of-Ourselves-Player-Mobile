using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionSetter : MonoBehaviour {

    [SerializeField] Vector2 resolution;

    private void Start()
    {
        Screen.SetResolution((int)resolution.x, (int)resolution.y, false);
    }

}
