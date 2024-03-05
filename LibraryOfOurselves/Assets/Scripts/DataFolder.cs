using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataFolder 
{
#if !UNITY_EDITOR && UNITY_ANDROID
	public static string UserPath = "/storage/emulated/0/Movies/LibraryOfOurselvesContent"; //TODO add filebrowser to be able to set that folder manually
#endif
#if UNITY_EDITOR 
    public static string GuidePath = Application.persistentDataPath;
    public static string UserPath = Application.persistentDataPath;
#endif
}
