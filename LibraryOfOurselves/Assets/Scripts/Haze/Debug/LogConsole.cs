using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LogConsole : MonoBehaviour
{

    static LogConsole instance = null;

    [SerializeField] string path = "log.txt";
	[SerializeField] float writeLogsTimer = 0;//every X seconds, write out the logs
	[SerializeField] int writeLogsCounter = 0;//every X lines written, write out the logs
    
    string t = "Console:\n";

	int linesWrittenSinceEpoch = 0;

    void OnEnable()
    {
        if (instance == null) instance = this;
        else gameObject.SetActive(false);

        Application.logMessageReceived += HandleLog;

		if(writeLogsTimer > 0)
			StartCoroutine(writeLogsClock());
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

        t += "[" + DateTime.Now + "] {" + type + "} " + message + "\n" + stackTrace + "\n";
		++linesWrittenSinceEpoch;

		if(writeLogsCounter > 0 && linesWrittenSinceEpoch >= writeLogsCounter)
			write();

	}

    public static string Logs()
    {
        return instance.t;
    }

    void write() {
		if(linesWrittenSinceEpoch <= 0) return;//no need

		string fullPath = Application.persistentDataPath;
		if(fullPath[fullPath.Length-1] != '/' && fullPath[fullPath.Length-1] != '\\') {
			fullPath += "/";
		}
		//fullPath += path;

		Debug.Log("Writing log file to: " + fullPath + path);

		string rf = t;
		rf.Replace("\n", "\r\n");

		//File.WriteAllText(fullPath, rf);
		FileWriter.WriteFile(fullPath, path, rf);

		linesWrittenSinceEpoch = 0;

	}

	IEnumerator writeLogsClock() {
		while(true) {
			yield return new WaitForSecondsRealtime(writeLogsTimer);
			write();
		}
	}

}
