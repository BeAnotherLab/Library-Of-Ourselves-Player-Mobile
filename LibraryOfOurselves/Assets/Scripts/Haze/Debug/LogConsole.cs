using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        t += message + "\n";
    }

    public static string Logs()
    {
        return instance.t;
    }

    void write()
    {
        using (FileStream fs = File.OpenWrite(path)){
            string rf = t;
            rf.Replace("\n", "\r\n");
            byte[] dat = new System.Text.UTF8Encoding(true).GetBytes(rf);
            fs.Write(dat, 0, dat.Length);
        }

    }

}
