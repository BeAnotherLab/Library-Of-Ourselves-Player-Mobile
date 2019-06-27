using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LogConsole : MonoBehaviour
{

    static LogConsole instance = null;

    [SerializeField] string path = "log.txt";
    
    string t = "Console:\n";

    void OnEnable()
    {
        if (instance == null) instance = this;
        else gameObject.SetActive(false);

        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        if (instance == this) instance = null;

        Application.logMessageReceived -= HandleLog;

        write();
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (!enabled) return;

        t += "[" + DateTime.Now + "] [" + type + "] " + message + "\n" + stackTrace + "\n";
    }

    public static string Logs()
    {
        return instance.t;
    }

    void write() {
		string fullPath = Application.persistentDataPath;
		if(fullPath[fullPath.Length-1] != '/' && fullPath[fullPath.Length-1] != '\\') {
			fullPath += "/";
		}
		fullPath += path;
		Debug.Log("Outputting logs to: " + fullPath);
		string rf = t;
		rf.Replace("\n", "\r\n");
		File.WriteAllText(fullPath, rf);

	}

}
